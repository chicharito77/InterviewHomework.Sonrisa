# DailyBugle — Decision Log

> Companion docs: [PLAN.md](./PLAN.md) · [ARCHITECTURE.md](./ARCHITECTURE.md)

One entry per significant decision made during planning. Format: Context → Decision → Rationale →
Alternatives Considered → Status.

---

## D-001: Documentation naming — `DECISION_LOG.md`

- **Context:** User referred to "DECISIONS_LOG"; existing `.github/copilot-instructions.md` (§9)
  specifies `DECISION_LOG.md`.
- **Decision:** Use `DECISION_LOG.md` (this file).
- **Rationale:** Consistency with the already-established repository instructions (single source of
  truth); avoids two conflicting naming conventions.
- **Alternatives considered:** `DECISIONS_LOG.md` (rejected — conflicts with existing convention).
- **Status:** ✅ Applied

## D-002: Persistence — in-memory only, no database

- **Context:** 6-hour time budget; brief does not require durable storage.
- **Decision:** Repository pattern backed by thread-safe in-memory collections
  (`ConcurrentDictionary`), no EF Core / SQL setup.
- **Rationale:** Eliminates DB provisioning/migration overhead; repository interfaces still allow a
  real persistence layer to be swapped in later without touching Domain/Engine/UI.
- **Alternatives considered:** EF Core InMemory provider (rejected — adds package/config overhead for
  no functional benefit at this stage); real SQL Server/SQLite (rejected — out of time budget).
- **Status:** ✅ Applied

## D-003: Notification delivery — real Email (Gmail SMTP) + real Slack (Incoming Webhook)

- **Context:** Asked whether channels should be simulated/logged only or perform real external
  delivery.
- **Decision:** Real delivery. Email via Gmail SMTP using an App Password; Slack via a real Incoming
  Webhook URL supplied by the user.
- **Rationale:** User explicitly chose real delivery to make the POC demonstrably convincing rather
  than simulated; both are cheap to wire up (no paid infra needed) and directly exercise the Strategy
  pattern end-to-end.
- **Alternatives considered:** Simulated/logged channels (rejected — user wants live validation);
  third-party SMTP test service (Mailtrap/Ethereal) for email (rejected in favor of the user's own
  Gmail account with App Password, so the demo delivers to a real inbox).
- **Risk carried forward:** Requires secrets (App Password, webhook URL) that must never be committed;
  see D-009.
- **Status:** ✅ Applied

## D-004: User model — full `User` entity owning multiple `AlertRule`s

- **Context:** Admin tab needs a "list of users"; needed to decide if `User` is a first-class domain
  entity or if rules just carry raw contact info.
- **Decision:** `User` is a full entity (`Id`, `Name`, `Email`, `SlackWebhookUrl`) with one-to-many
  ownership of `AlertRule`s.
- **Rationale:** Matches the requirement more naturally (Admin genuinely lists users, not raw
  strings); supports future features (per-user channel targets, multiple rules per user) without
  redesign.
- **Alternatives considered:** No `User` entity, rules carry channel target directly (rejected — would
  need ad hoc string-based grouping for the Admin "users" list, weaker domain model).
- **Status:** ✅ Applied

## D-005: NewsSimulator triggering — manual, single-shot only

- **Context:** Decide whether the simulator should support automatic/periodic event generation.
- **Decision:** Manual only — Admin selects a `NewsType` (+ optional payload) and clicks "Fire Event"
  to inject exactly one `Event`.
- **Rationale:** Simpler to build and demo within the time budget; deterministic for manual testing
  and for future automated tests; matches the brief's "admin view" framing.
- **Alternatives considered:** Timer-based random event generation (deferred to backlog — see
  PLAN.md §3 out-of-scope).
- **Status:** ✅ Applied

## D-006: AlertRule granularity — NewsType + Channel only

- **Context:** Decide whether `AlertRule` should support severity/keyword filtering in addition to
  news type.
- **Decision:** `AlertRule` = `User` + `NewsType` + `ChannelType` only. No severity/keyword filter for
  this POC.
- **Rationale:** Minimizes scope to fit the 6h budget while still exercising the full
  Observer→Strategy dispatch flow; filtering is a straightforward, isolated future extension (single
  predicate addition in `AlertDispatcher`, see ARCHITECTURE.md §6).
- **Alternatives considered:** Severity/keyword filtering now (rejected — adds domain and UI
  complexity disproportionate to remaining time).
- **Status:** ✅ Applied

## D-007: Microsoft Teams channel — documented only, not implemented

- **Context:** Brief and instructions call for "easily expandable... new notification type (teams)".
- **Decision:** Document the extension path in ARCHITECTURE.md (§6) but do not implement a
  `TeamsNotificationChannel` in this POC.
- **Rationale:** Implementing Email + Slack already proves the Strategy pattern's plug-and-play
  nature; a third channel adds implementation time without adding architectural evidence. Time is
  better spent on tests and polish.
- **Alternatives considered:** Stub/no-op `TeamsNotificationChannel` for a live demonstration
  (rejected by user — chose documentation-only to save time).
- **Status:** ✅ Applied

## D-008: Admin tab scope — users list + fire-event controls only

- **Context:** Decide what "system monitoring" means in the Admin tab.
- **Decision:** Admin tab shows the users list and fire-event controls only; no notification
  history/delivery-log view.
- **Rationale:** Keeps Phase 3 (frontend) within budget; a monitoring log is a natural but
  non-essential addition, deferred to backlog.
- **Alternatives considered:** Add a dispatched-notifications log (timestamp/user/channel/success)
  (rejected for time — see PLAN.md §3 out-of-scope).
- **Status:** ✅ Applied

## D-009: Secrets handling — real credentials never committed

- **Context:** User provided a real Slack Incoming Webhook URL and personal Gmail address/App
  Password intent directly in chat for today's local testing only.
- **Decision:** These values are used only for local runtime configuration (e.g.
  `appsettings.Development.json` or .NET user-secrets, gitignored) and are never written into source
  files, committed to git, or included in any documentation/commit message.
- **Rationale:** Security/privacy best practice; prevents credential leakage into a submitted public
  interview repository.
- **Alternatives considered:** None — this is a hard constraint, not a trade-off.
- **Status:** ✅ Applied (ongoing — must be verified again during Phase 2 implementation and Phase 5
  final repo scan)

## D-009a: Secrets storage hardened — encrypted at rest, not just gitignored

- **Context:** User pointed out that keeping secrets out of git is not sufficient — even local,
  non-committed files should not hold plaintext credentials.
- **Decision:** Real credentials (Gmail App Password, Slack Incoming Webhook URL, and seed users'
  contact addresses) are encrypted at rest using Windows DPAPI
  (`System.Security.Cryptography.ProtectedData`, `DataProtectionScope.CurrentUser`) via a small
  `SecretProtector` helper in `DailyBugle.Infrastructure`. Ciphertext is stored in a local,
  gitignored `secrets.local.json`; plaintext values only ever exist in memory at runtime after
  decryption. No plaintext secret is ever written to a committed file, log, or doc.
- **Rationale:** DPAPI requires no extra credential/key management (tied to the Windows user
  account) and needs no third-party dependency — fits the time budget while meaningfully raising
  the bar over "just gitignored plaintext".
- **Alternatives considered:** Windows Credential Manager (equally valid, slightly more ceremony via
  P/Invoke/NuGet wrapper — deferred as unnecessary for POC scope); Azure Key Vault / external secret
  manager (rejected — requires external infra, out of scope).
- **Status:** ✅ Applied (amends D-009)

## D-010: Solution/project layout — Clean Architecture, one solution under `src/`

- **Context:** Needed a concrete project split satisfying Clean Architecture + SOLID + testability
  requirements from `.github/copilot-instructions.md`.
- **Decision:** `DailyBugle.sln` with `Domain`, `Notifications`, `Engine`, `Infrastructure`, `Wpf`,
  `Tests` projects (see ARCHITECTURE.md §2).
- **Rationale:** Domain has zero external dependencies (pure, fast unit tests); channels and
  persistence depend only on Domain abstractions (Dependency Inversion); Engine orchestrates without
  knowing about WPF; WPF composes everything at startup (composition root). This directly supports
  the "easily expandable" and "unit-test friendly" requirements.
- **Alternatives considered:** Single monolithic project (rejected — harder to enforce dependency
  direction and test isolation); separate `tests/` folder at repo root (rejected — instructions state
  all implementation, including tests, lives under `src/`).
- **Status:** ✅ Applied

## D-011: Seed/test users — two fixed demo users, no registration UI

- **Context:** User supplied two concrete test personas and asked to confirm there is no
  user-registration form in this POC.
- **Decision:** Confirmed — no registration/user-creation UI. Two `User`s are seeded at startup in
  `DailyBugle.Infrastructure` (a small seeding routine populating `InMemoryUserRepository` /
  `InMemoryAlertRuleRepository`):
  - **User 1 — Németh István**: channel `Email`, subscribed to `NewsType.Sport` **and**
    `NewsType.Science` (two `AlertRule`s). Recipient address is the same account used as the app's
    outgoing sender (self-test loop).
  - **User 2 — Estebán Alemán**: channel `Slack`, subscribed to `NewsType.Science` **and**
    `NewsType.Economy` (two `AlertRule`s). Uses the provided Slack Incoming Webhook URL.
  Both users share the `Science` interest deliberately — firing a Science event must notify both
  (via their respective channels) in the same dispatch cycle, giving a concrete manual test for
  "multiple users notified about the same event."
  Actual contact values (email address, webhook URL) are supplied via the encrypted local secret
  store (D-009a) at runtime — never hardcoded in source or committed docs, only structural seed
  metadata (name, news type, channel) is committed.
- **Rationale:** Matches the "no time for registration form" constraint while still giving the Admin
  tab real users to list and the User tab real rules to manage/demo end-to-end delivery.
- **Alternatives considered:** Registration form (rejected — out of time budget, added to backlog).
- **Status:** ✅ Applied

## D-012: User tab — per-user notification history panel added

- **Context:** After reviewing the plan, user asked for a history list in the User tab showing
  occurred events relevant to them.
- **Decision:** Add a `DeliveryRecord` entity (`Id`, `EventId`, `UserId`, `Channel`, `OccurredAt`,
  `Success`, `ErrorMessage?`) + `IDeliveryRecordRepository` / `InMemoryDeliveryRecordRepository`.
  `AlertDispatcher` writes one `DeliveryRecord` per dispatch attempt (success or failure). The User
  tab queries records filtered by the acting user's Id.
- **Rationale:** Directly requested; low incremental cost since `AlertDispatcher` already knows the
  outcome of each `SendAsync` call — just needs to persist it. Distinct from D-008 (Admin
  cross-user monitoring log, still explicitly out of scope) — this is scoped to a single user's own
  history, driven by the User tab's own need, not a general admin monitoring feature.
- **Alternatives considered:** Reuse an admin-wide log and filter client-side (rejected — conflates
  the two concerns; a dedicated per-user-queryable repository is cleaner and just as fast to build).
- **Status:** ✅ Applied — supersedes the "no history" default in D-008 only for the **User** tab;
  Admin tab still has no monitoring log.

## D-013: Global "Acting as" identity switcher — Admin is a UI-only pseudo-identity

- **Context:** Initial wireframe put an "Acting as" selector *inside* the User tab, alongside
  always-visible Admin/User tabs. User clarified there are really **3 acting identities** (Admin +
  2 seeded demo users) and wants a single **global** switcher (top-right of the main window) whose
  selection determines which tab is visible at all — Admin selected ⇒ only Admin tab shown; a demo
  user selected ⇒ only User tab shown, scoped to that user's own rules/history.
- **Decision:** `MainViewModel` owns one `ActingAs` selection with 3 options: `Admin` (fixed,
  UI-only — no `Email`/`SlackWebhookUrl`/`AlertRule`s, **not** stored via `IUserRepository`) and the
  2 seeded `User` entities. Tab visibility (not just tab selection) is bound to this value; the
  User tab's data context switches to the selected demo user. See ARCHITECTURE.md §10 for the
  revised wireframes.
- **Rationale:** Matches the user's explicit mental model exactly; keeps `Admin` out of the domain
  model entirely (it has no email, no Slack target, no rules — modeling it as a `User` would force
  meaningless nulls/flags), which is cleaner and avoids polluting `IUserRepository` with a
  non-subscriber pseudo-entity.
- **Alternatives considered:** Per-tab "Acting as" selector with both tabs always visible (rejected —
  doesn't match the requested UX, and always-visible tabs for identities that shouldn't see each
  other's view is confusing); modeling Admin as a `User` with an `IsAdmin` flag (rejected — adds
  domain complexity/nullable fields for a role that has no subscriber attributes).
- **Status:** ✅ Applied — supersedes the "acting-as inside User tab" framing from the original
  wireframe draft.

## D-014: Seed rules expanded — shared Science interest to test multi-user dispatch

- **Context:** User wants a concrete manual test proving one fired event can notify multiple users
  across different channels in a single dispatch.
- **Decision:** Amend D-011's seed rules: Németh István gains a second `AlertRule`
  (`NewsType.Science`, `Email`) in addition to `Sport`; Estebán Alemán gains a second `AlertRule`
  (`NewsType.Economy`, `Slack`) in addition to `Science`. Both users now share `Science` as a common
  interest; `Sport` remains unique to Németh and `Economy` unique to Estebán.
- **Rationale:** Firing a `Science` event now exercises the full multi-recipient path — `AlertDispatcher`
  must resolve two matching `AlertRule`s owned by two different `User`s and dispatch through two
  different `INotificationChannel` strategies (Email + Slack) from one `Event`. `Sport`/`Economy`
  remain single-recipient controls to also verify the "only the right user is notified" case.
- **Alternatives considered:** Keep one NewsType per user (rejected — user explicitly wants to
  validate the multi-user-same-event path, which requires an overlapping interest).
- **Status:** ✅ Applied — amends D-011 seed data (see ARCHITECTURE.md §9 for the updated table).
