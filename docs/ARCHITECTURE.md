# DailyBugle — Architecture

> Companion docs: [PLAN.md](./PLAN.md) · [DECISION_LOG.md](./DECISION_LOG.md)

## 1. Overview

DailyBugle is a POC alerting system: a simulated news feed (`NewsSimulator`) publishes `Event`s,
an `AlertDispatcher` matches them against user-defined `AlertRule`s, and matching alerts are sent
through a pluggable `INotificationChannel` (Strategy pattern) — currently **Email (SMTP)** and
**Slack (Incoming Webhook)**. A WPF (MVVM) front end exposes an Admin view (fire events, list users)
and a User view (manage subscription rules).

## 2. Solution Layout

```
src/
  DailyBugle.sln
  DailyBugle.Domain/            Entities, enums, domain interfaces (no external deps)
    Entities/
      Event.cs
      AlertRule.cs
      User.cs
    Enums/
      NewsType.cs
      ChannelType.cs
    Abstractions/
      IDateTimeProvider.cs       (DateTimeHandler contract — testable clock)
      IUserRepository.cs
      IAlertRuleRepository.cs
      IDeliveryRecordRepository.cs
      IEventPublisher.cs         (Observer/Pub-Sub contract)
    Exceptions/
      UserNotFoundException.cs
      InvalidNotificationChannelException.cs
      ...

  DailyBugle.Notifications/     Strategy implementations for channels
    INotificationChannel.cs
    EmailNotificationChannel.cs  (Gmail SMTP via MailKit/System.Net.Mail)
    SlackNotificationChannel.cs  (HttpClient POST to Incoming Webhook)
    Configuration/
      EmailChannelOptions.cs
      SlackChannelOptions.cs

  DailyBugle.Engine/             Application/orchestration layer
    NewsSimulator.cs             (Observable — manual single-shot event firing)
    AlertDispatcher.cs           (Observer — matches Events -> AlertRules -> Channels)
    NotificationChannelResolver.cs (Strategy selector: ChannelType -> INotificationChannel)
    Services/
      UserService.cs
      AlertRuleService.cs

  DailyBugle.Infrastructure/      In-memory persistence
    DateTimeHandler.cs            (IDateTimeProvider impl, UtcNow wrapper)
    Security/
      SecretProtector.cs          (DPAPI encrypt/decrypt helper, see D-009a)
    Repositories/
      InMemoryUserRepository.cs   (ConcurrentDictionary-backed)
      InMemoryAlertRuleRepository.cs
      InMemoryDeliveryRecordRepository.cs

  DailyBugle.Wpf/                 Presentation (MVVM)
    App.xaml / App.xaml.cs        (DI composition root)
    Views/
      MainWindow.xaml              (hosts "Acting as" switcher + conditional TabControl)
      AdminView.xaml
      UserView.xaml
    ViewModels/
      MainViewModel.cs             (owns ActingAs identity list + tab visibility switch)
      AdminViewModel.cs
      UserViewModel.cs
    appsettings.json               (non-secret defaults only — no credentials)
    secrets.local.json             (gitignored — DPAPI-encrypted ciphertext, see D-009a; not plaintext)

  DailyBugle.Tests/                NUnit + Moq
    AlertDispatcherTests.cs
    AlertRuleMatchingTests.cs
    NotificationChannelResolverTests.cs
```

**Why this split:** each project is a Clean-Architecture ring. `Domain` has zero external dependencies
(pure C#), `Notifications` and `Infrastructure` depend only on `Domain` abstractions (Dependency
Inversion), `Engine` orchestrates domain + notifications, and `Wpf` composes everything at startup.
This keeps the domain unit-testable in isolation and channels swappable without touching the engine.

## 3. Domain Model

```
User
 ├─ Id            : Guid
 ├─ Name          : string
 ├─ Email         : string            (target for EmailNotificationChannel)
 ├─ SlackWebhookUrl : string?         (target for SlackNotificationChannel; per-user, optional)
 └─ AlertRules    : IReadOnlyCollection<AlertRule>  (navigation, resolved via repository)

AlertRule
 ├─ Id          : Guid
 ├─ UserId      : Guid
 ├─ NewsType    : NewsType            (Sport | Economy | Science | ...)
 ├─ Channel     : ChannelType         (Email | Slack | ...)
 ├─ IsActive    : bool
 └─ CreatedAt   : DateTime            (via IDateTimeProvider)

Event
 ├─ Id           : Guid
 ├─ NewsType     : NewsType
 ├─ Title        : string
 ├─ Description  : string
 └─ OccurredAt   : DateTime           (via IDateTimeProvider)

DeliveryRecord
 ├─ Id           : Guid
 ├─ EventId      : Guid
 ├─ UserId       : Guid
 ├─ Channel      : ChannelType
 ├─ OccurredAt   : DateTime           (via IDateTimeProvider)
 ├─ Success      : bool
 └─ ErrorMessage : string?           (populated when Success = false)
```

**Design note:** `DeliveryRecord` backs the **User tab's** per-user notification history panel only
(D-012) — it is not a general admin-wide monitoring feature (that remains explicitly out of scope,
D-008). `AlertDispatcher` writes exactly one `DeliveryRecord` per dispatch attempt, whether it
succeeds or fails.

**Design note:** per-user Slack webhook is modeled as optional on `User` for realism (each Slack
workspace/user could have a distinct webhook), but for this POC only the one Slack-channel demo
user (Estebán Alemán) is actually configured with a webhook — Németh István (Email-only) leaves
`SlackWebhookUrl` unset. `AlertRule` intentionally does **not** carry a severity/keyword filter —
matching is NewsType-only (see DECISION_LOG.md, "AlertRule granularity").

## 4. Design Patterns & Where They Live

| Pattern | Purpose | Location |
|---|---|---|
| **Strategy** | Interchangeable notification delivery | `INotificationChannel` + `EmailNotificationChannel`/`SlackNotificationChannel`; selected via `NotificationChannelResolver` (`ChannelType` → implementation) |
| **Observer / Pub-Sub** | Decouple event *production* from *consumption* | `NewsSimulator` implements `IEventPublisher` (raises events); `AlertDispatcher` subscribes and reacts — `NewsSimulator` has no knowledge of `AlertDispatcher` |
| **Repository** | Abstract persistence, swappable later (e.g., EF Core) | `IUserRepository` / `IAlertRuleRepository` (Domain) + `InMemory*Repository` (Infrastructure) |
| **MVVM** | WPF presentation separation | `Views/` (XAML, no logic) ↔ `ViewModels/` (bindable state + commands) ↔ Engine/Domain (no UI awareness) |
| **Dependency Injection** | Testability, loose coupling, SOLID (DIP) | Composition root in `App.xaml.cs` using `Microsoft.Extensions.DependencyInjection` |

## 5. Core Flow — "Fire Event" (Admin) to Delivery

```
Admin Tab                NewsSimulator            AlertDispatcher         AlertRuleRepo      NotificationChannelResolver     Channel (Email/Slack)
   │  click "Fire Event"      │                          │                     │                       │                          │
   ├── Publish(NewsType,──────▶                          │                     │                       │                          │
   │   title, description)    │  raises OnEventPublished │                     │                       │                          │
   │                          ├─────────────────────────▶│                     │                       │                          │
   │                          │                          ├── GetActiveRulesFor(NewsType) ──────────────▶│                       │
   │                          │                          │◀───── matching AlertRules ────────────────────┤                       │
   │                          │                          ├── Resolve(rule.Channel) ─────────────────────────────────────────────▶│
   │                          │                          │◀──── INotificationChannel instance ──────────────────────────────────┤
   │                          │                          ├── SendAsync(user, event) ─────────────────────────────────────────────▶│
   │                          │                          │                     │                       │      (SMTP send / Slack POST)
```

Each matching `AlertRule` triggers exactly one `SendAsync` call on the resolved channel, followed by
one `DeliveryRecord` write (success or failure) via `IDeliveryRecordRepository` — this is what backs
the User tab's history panel (D-012). Failures are also caught, logged with context (rule id, user
id, channel), and do **not** stop processing of remaining rules (fail-fast for programming errors,
resilient for delivery errors — see DECISION_LOG.md).

## 6. Extensibility

- **New NewsType** (e.g., `Politics`): add enum member; no changes needed to `AlertDispatcher`,
  channels, or repositories — purely additive.
- **New Channel** (e.g., Microsoft Teams): implement `INotificationChannel`, register in
  `NotificationChannelResolver`'s DI registration. **Documented but not implemented in this POC**
  (see DECISION_LOG.md) — would follow the exact same shape as `SlackNotificationChannel` (webhook
  POST), so no dispatcher/domain changes required. This is the acceptance test for the Strategy
  pattern's value: adding a channel must never touch `AlertDispatcher`.
- **Future filtering** (severity/keyword): would extend `AlertRule` with optional filter fields and
  add a predicate check in `AlertDispatcher`'s matching step — isolated to one method.
- **Future persistence**: swapping `InMemoryUserRepository` for an EF Core-backed implementation
  requires no change to `Domain`, `Engine`, or `Wpf` — only a DI registration change.

## 7. Testability Strategy

- `DateTimeHandler` (`IDateTimeProvider`) wraps `DateTime.UtcNow` so tests can inject fixed/fake clocks.
- `INotificationChannel` is mocked (Moq) in dispatcher tests — no real SMTP/Slack calls in unit tests.
- Repositories are interfaces — tests can substitute in-memory fakes or Moq mocks as needed.
- Planned fixtures (Phase 4, ≥3): `AlertDispatcher` correctly matches rules & invokes only the
  resolved channel; `AlertRule`/matching edge cases (inactive rule, no matching rule, multiple
  matching rules for same user); `NotificationChannelResolver` throws
  `InvalidNotificationChannelException` for unregistered channel types.

## 8. Secrets & Configuration

- Gmail App Password, Slack Incoming Webhook URL, and seed users' real contact addresses are
  **never** committed to source control and **never** stored as plaintext, even locally.
- Encrypted at rest via Windows DPAPI (`ProtectedData`, `CurrentUser` scope) through a
  `SecretProtector` helper in `DailyBugle.Infrastructure`; ciphertext lives in a gitignored
  `secrets.local.json`. Plaintext only exists transiently in memory after decryption at startup
  (see DECISION_LOG.md D-009a).
- `EmailChannelOptions` / `SlackChannelOptions` are populated from the decrypted secrets at startup
  (Options pattern) and injected into the respective channel — no hardcoded credentials anywhere in
  source.
- Missing/invalid configuration fails fast at startup with a clear exception rather than silently
  no-op-ing at send time.

## 9. Seed Data

Two demo `User`s + four `AlertRule`s total are seeded at startup by a seeding routine in
`DailyBugle.Infrastructure` (populates `InMemoryUserRepository` / `InMemoryAlertRuleRepository`
directly — no registration UI, see DECISION_LOG.md D-011/D-014):

| User | Channel | Subscribed NewsTypes | Contact target |
|---|---|---|---|
| Németh István | Email | Sport, **Science** | resolved from encrypted secret store (self-test: sender == recipient) |
| Estebán Alemán | Slack | **Science**, Economy | resolved from encrypted secret store (provided webhook) |

`Science` is deliberately shared by both users — firing a `Science` event must dispatch to **both**
(Email to Németh, Slack to Estebán) in one cycle, giving a concrete manual test for multi-user
same-event dispatch. `Sport` (Németh only) and `Economy` (Estebán only) act as single-recipient
controls to verify only the correct user is notified for those types.

Only structural seed metadata (name, news type, channel) is committed; actual addresses/webhook
URLs are never hardcoded in source or docs.

## 10. UI Wireframes (ASCII, indicative only)

### Identity model

There are **3 acting identities**, selected via a single global "Acting as" dropdown in the
top-right corner of the main window (not inside a tab):

| Identity | Domain entity? | Visible tab(s) |
|---|---|---|
| **Admin** | No — UI-only pseudo-identity, not stored in `IUserRepository`, has no `Email`/`SlackWebhookUrl`/`AlertRule`s | Admin tab only |
| **Németh István** | Yes — seeded `User` | User tab only |
| **Estebán Alemán** | Yes — seeded `User` | User tab only |

Switching identity toggles which tab is **visible** (not just which is selected) — selecting Admin
hides the User tab entirely; selecting either demo user hides the Admin tab and loads that user's
own rules/history into the User tab. `MainViewModel` owns this switch (`IsAdminModeActive` /
`ActingUser`); `TabControl` visibility is bound to it.

**Main window frame — Admin identity selected:**

```
┌─────────────────────────────── DailyBugle ─────────────────────────────────┐
│                                                      Acting as: [ Admin ▾ ]  │
├─────────────────────────────────────────────────────────────────────────┤
│  ┌────────┐                                                                │
│  │ Admin  │   ← only tab shown while acting as Admin                      │
│  └────────┴──────────────────────────────────────────────────────────┐    │
│  Registered Users                                                     │   │
│  ┌───────────────────────────────────────────────────────────────┐   │   │
│  │ Name              │ Channel │ Subscribed News Types            │   │   │
│  ├───────────────────────────────────────────────────────────────┤   │   │
│  │ Németh István      │ Email   │ Sport, Science                 │   │   │
│  │ Estebán Alemán     │ Slack   │ Science, Economy                │   │   │
│  └───────────────────────────────────────────────────────────────┘   │   │
│                                                                        │   │
│  Fire Event                                                           │   │
│  News Type:  [ Science ▾ ]                                            │   │
│  Title:       [_____________________________________________]        │   │
│  Description: [_____________________________________________]        │   │
│                                          [ Fire Event ]                │   │
│                                                                        │   │
│  Last Dispatch Results                                                │   │
│  ┌───────────────────────────────────────────────────────────────┐   │   │
│  │ User              │ Channel │ Status                          │   │   │
│  ├───────────────────────────────────────────────────────────────┤   │   │
│  │ Németh István      │ Email   │ ✔ Delivered                    │   │   │
│  │ Estebán Alemán     │ Slack   │ ✔ Delivered                    │   │   │
│  └───────────────────────────────────────────────────────────────┘   │   │
│                                                                        │   │
└────────────────────────────────────────────────────────────────────┴───┘
```

Firing a `Science` event (as shown) is the concrete manual test for multi-user, multi-channel
dispatch from a single `Event` (D-014); firing `Sport` or `Economy` instead demonstrates
single-recipient dispatch.

**Main window frame — "Németh István" identity selected** (same shell, Admin tab now hidden,
User tab shown with that user's own data):

```
┌─────────────────────────────── DailyBugle ─────────────────────────────────┐
│                                       Acting as: [ Németh István ▾ ]        │
├─────────────────────────────────────────────────────────────────────────┤
│  ┌────────┐                                                                │
│  │ User   │   ← only tab shown while acting as a non-admin identity        │
│  └────────┴──────────────────────────────────────────────────────────┐    │
│  My Alert Rules                                                       │   │
│  ┌───────────────────────────────────────────────────────────────┐   │   │
│  │ News Type   │ Channel │ Active │                               │   │   │
│  ├───────────────────────────────────────────────────────────────┤   │   │
│  │ Sport       │ Email   │  ✔    │        [ Remove ]              │   │   │
│  │ Science     │ Email   │  ✔    │        [ Remove ]              │   │   │
│  └───────────────────────────────────────────────────────────────┘   │   │
│                                                                        │   │
│  Add New Rule                                                         │   │
│  News Type: [ Economy ▾ ]     Channel: [ Email ▾ ]                    │   │
│                                            [ Add Rule ]                │   │
│                                                                        │   │
│  My Notification History                                              │   │
│  ┌───────────────────────────────────────────────────────────────┐   │   │
│  │ When       │ News Type │ Channel │ Status                      │   │   │
│  ├───────────────────────────────────────────────────────────────┤   │   │
│  │ 11:42:03   │ Science   │ Email   │ ✔ Delivered                 │   │   │
│  │ 11:30:11   │ Sport     │ Email   │ ✘ Failed: SMTP timeout       │   │   │
│  └───────────────────────────────────────────────────────────────┘   │   │
│                                                                        │   │
└────────────────────────────────────────────────────────────────────┴───┘
```

Selecting "Estebán Alemán" renders the same User tab shell, scoped to his own rules/history
(Slack/Science, Slack/Economy) instead.

## 11. Out of Scope for This POC (see PLAN.md §3)

Teams channel implementation, automatic/periodic simulation, severity/keyword rule filtering, admin
notification history/monitoring log, durable persistence, authentication.
