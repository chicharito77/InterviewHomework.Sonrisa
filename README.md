# InterviewHomework.Sonrisa

**DailyBugle** — a proof-of-concept alerting system built from a deliberately vague product brief
(see [`docs/task.txt`](./docs/task.txt)): users subscribe to "important world events" (simulated
Sport/Economy/Science news) and get notified over **Email (Gmail SMTP)** or **Slack (Incoming
Webhook)**, with an Admin view to fire events and a User view to manage subscriptions and see
delivery history.

This repository is submitted as **evidence of an AI-directed engineering process**, not just a
finished app — see [What we're evaluating](./docs/task.txt) for the brief's own framing. The
`docs/` folder is the primary artifact: plans, decisions, prompts, and test reports produced while
directing the AI agent through each phase.

## Status

| Phase | Deliverable | Status |
|---|---|---|
| 1. Planning & Architecture | [`PLAN.md`](./docs/PLAN.md), [`ARCHITECTURE.md`](./docs/ARCHITECTURE.md), [`DECISION_LOG.md`](./docs/DECISION_LOG.md) | ✅ Done |
| 2. Backend implementation | `src/DailyBugle.Domain`, `.Notifications`, `.Engine`, `.Infrastructure` | ✅ Done — validated via manual smoke tests (real Gmail/Slack delivery, no mocks) |
| 3. Frontend implementation | `src/DailyBugle.Wpf` (MVVM, Admin + User tabs) | ✅ Done — validated via automated UI-automation smoke test + human manual test pass |
| 4. Automated unit tests (NUnit/Moq) | `src/DailyBugle.Tests` | ⬜ **Skipped** — ran out of time budget; see [Testing approach](#testing-approach-why-unit-tests-are-missing) below for what substituted for it |
| 5. Documentation pass | This README + cross-links | ✅ Done (this pass) |

Full phase-by-phase reasoning: [`docs/PLAN.md`](./docs/PLAN.md) §2, §5 (time allocation and the
explicit cut order when the budget compressed).

## Repository Layout

```
.github/copilot-instructions.md   Engineering standards & workflow rules the AI agent followed
docs/
  task.txt                        Original brief, verbatim
  PLAN.md                         Phased plan, scope decisions, time budget, acceptance criteria
  ARCHITECTURE.md                 Solution layout, domain model, design patterns, data flow, extensibility
  DECISION_LOG.md                 One entry per significant decision (context → decision → rationale → alternatives)
  FUTURE_IMPROVEMENTS.md          Known rough edges / backlog, explicitly deferred past the time budget
  testreports/                    Manual & automated test evidence (Phase 2 smoke tests, Phase 3 UI tests), screenshots in testreports/images/
  prompts/                        Prompt history per phase (SESSION_TRANSCRIPT.md, SUMMARY.md, USER_PROMPTS.md)
src/
  DailyBugle.sln                  Main solution (Domain, Notifications, Engine, Infrastructure, Wpf, Tests, SecretsTool)
  DailyBugle.Domain/               Entities (User, AlertRule, Event, DeliveryRecord), enums, abstractions — no external deps
  DailyBugle.Notifications/        Strategy pattern: INotificationChannel + Email/Slack implementations
  DailyBugle.Engine/                Orchestration: NewsSimulator (Observer/publisher), AlertDispatcher, NotificationChannelResolver, services
  DailyBugle.Infrastructure/        In-memory repositories, DateTimeHandler, DPAPI-based secret encryption (SecretProtector)
  DailyBugle.Wpf/                   WPF MVVM front end (Admin + User tabs, "Acting as" identity switcher)
  DailyBugle.Tests/                 NUnit/Moq test project — scaffolded, no fixtures written (Phase 4 skipped, see below)
  DailyBugle.SecretsTool/           Standalone console tool: encrypts local plaintext credentials into secrets.local.json
  DailyBugle.SmokeTest/             Standalone console harness for manual real-delivery smoke testing (not in .sln, see its own README)
```

Deep dives: [ARCHITECTURE.md](./docs/ARCHITECTURE.md) (solution layout §2, domain model §3, design
patterns §4, core flow §5, extensibility §6, secrets §8, seed data §9, UI wireframes §10) ·
[DECISION_LOG.md](./docs/DECISION_LOG.md) (why in-memory persistence, why real Email/Slack delivery,
why DPAPI-encrypted secrets, why no Teams channel yet, etc.).

## Running the Solution

**Prerequisites:** Windows (WPF + DPAPI-based secret encryption both require it), .NET 10 SDK.

### 1. Build

```powershell
cd src
dotnet build DailyBugle.sln
```

### 2. Provide credentials (required — the app fails fast without them)

The WPF app needs a Gmail App Password (for outgoing SMTP) and a Slack Incoming Webhook URL. These
are **never** committed and **never** stored as plaintext — they're encrypted at rest via Windows
DPAPI (see [`DECISION_LOG.md` D-009/D-009a](./docs/DECISION_LOG.md#d-009-secrets-handling--real-credentials-never-committed)).
`App.xaml.cs` intentionally throws and shows a startup error dialog if `secrets.local.json` is
missing/undecryptable — there is no silent fallback.

To generate it:

1. Create a plaintext file with these three `key=value` lines (see
   `DailyBugle.SecretsTool`'s own prompts for the exact required keys):
   ```
   GMAIL_SENDER_EMAIL=you@gmail.com
   GMAIL_APP_PASSWORD=xxxxxxxxxxxxxxxx
   SLACK_WEBHOOK_URL=https://hooks.slack.com/services/...
   ```
   (Gmail requires 2-Step Verification enabled and an
   [App Password](https://myaccount.google.com/apppasswords) — a normal account password will fail,
   see [`phase2-smoke-test-attempt1-gmail-auth-failed.md`](./docs/testreports/phase2-smoke-test-attempt1-gmail-auth-failed.md).)
2. Run the tool and follow its prompts:
   ```powershell
   cd src\DailyBugle.SecretsTool
   dotnet run
   ```
   It encrypts the values (DPAPI, current Windows user scope) into
   `src/DailyBugle.Wpf/secrets.local.json` and deletes your plaintext input file.

**Reviewers without a Gmail/Slack account to spare:** the app cannot start without valid,
decryptable secrets — this is by design (fail-fast, D-009a). If you just want to inspect delivery
behavior without running the live app, see the recorded evidence in
[`docs/testreports/`](./docs/testreports/) (screenshots + full request/response narratives) instead
of re-running live.

### 3. Run the app

```powershell
cd src\DailyBugle.Wpf
dotnet run
```

Two demo users are seeded at startup — no registration UI (see
[`DECISION_LOG.md` D-011/D-014](./docs/DECISION_LOG.md#d-011-seedtest-users--two-fixed-demo-users-no-registration-ui)
and [`ARCHITECTURE.md` §9](./docs/ARCHITECTURE.md#9-seed-data)). Use the "Acting as" switcher
(top-right) to jump between the Admin view (list users, fire an event) and each demo user's own
view (manage rules, see delivery history).

### Optional: console smoke-test harness

`DailyBugle.SmokeTest` (not part of `DailyBugle.sln`, see its own
[README](./src/DailyBugle.SmokeTest/README.md)) exercises the full backend pipeline —
secrets → seed data → `NewsSimulator` → `AlertDispatcher` → real Email/Slack delivery — without the
WPF UI. Useful for re-validating backend behavior after rotating credentials.

```powershell
cd src\DailyBugle.SmokeTest
dotnet run
```

## Testing Approach (why unit tests are missing)

Phase 4 (NUnit/Moq unit tests) was **cut** when the time budget compressed (see
[`PLAN.md` §5](./docs/PLAN.md#5-time-allocation-revised--hard-deadline-today-1530), which names
Phase 4 as the second thing to cut after Phase 5 polish). `DailyBugle.Tests` exists as a scaffolded,
wired-up project (NUnit + Moq + project references) but contains no fixtures.

In its place, correctness was validated three other ways, all documented with reasoning and
evidence in [`docs/testreports/`](./docs/testreports/):

1. **Phase 2 backend smoke tests** (real Gmail/Slack, no mocks) —
   [attempt 1 (failed, root-caused to a Gmail auth issue)](./docs/testreports/phase2-smoke-test-attempt1-gmail-auth-failed.md)
   and [attempt 2 (success, after the user fixed the Gmail App Password)](./docs/testreports/phase2-smoke-test-attempt2-success.md).
   These confirm `AlertDispatcher` rule-matching, `NotificationChannelResolver` strategy selection,
   and both channels' live delivery end-to-end.
2. **Phase 3 automated UI smoke test** —
   [round 1](./docs/testreports/phase3-frontend-smoke-test-round1.md): scripted build → launch →
   screenshot → UI-Automation pass, including a caught tooling pitfall (UI Automation returning
   peers for `Collapsed` elements) that was cross-checked against pixel screenshots before being
   trusted.
3. **Phase 3 human manual test pass** —
   [round 2](./docs/testreports/phase3-frontend-manual-test-round2.md): 4 sequential test cases
   (add/remove rules, multi-user/multi-channel dispatch, targeted exclusion) with Gmail/Slack
   screenshot evidence, plus 3 findings triaged and tracked in
   [`FUTURE_IMPROVEMENTS.md`](./docs/FUTURE_IMPROVEMENTS.md) (FI-002, FI-003) rather than fixed
   on the spot, per the documented scope-cut priority.

**Risk this carries forward:** no regression safety net for future changes to `AlertDispatcher`/
matching logic — a real gap, called out here rather than hidden. First follow-up recommendation if
this became a real project: write the ≥3 fixtures originally planned in
[`PLAN.md` §6](./docs/PLAN.md#6-acceptance-criteria-per-phase) (dispatcher matching/dispatch, rule
edge cases, channel-resolver error path) before adding new features.

## AI Tooling Cost

Built with GitHub Copilot Pro (subscribed the day before this task). Usage for the full 5-phase
session: all included "normal" premium requests consumed, plus **363/1000** requests from the
additional paid budget.

## Known Limitations / Backlog

See [`docs/FUTURE_IMPROVEMENTS.md`](./docs/FUTURE_IMPROVEMENTS.md) for rough edges found during
testing (e.g., Admin tab's user list not live-updating, "Active" checkbox not yet wired to a toggle
command) and [`docs/PLAN.md` §3](./docs/PLAN.md#3-scope-decisions-see-decision_logmd-for-full-rationale)
for features explicitly out of scope by design (Teams channel, severity/keyword filtering,
persistent storage, authentication).

## Process Evidence

- [`docs/DECISION_LOG.md`](./docs/DECISION_LOG.md) — every non-trivial choice, with rejected
  alternatives and rationale.
- [`docs/prompts/`](./docs/prompts/) — prompt history per phase (`phase1-planning/`,
  `phase2-backend/`, `phase3-frontend/`), each with `USER_PROMPTS.md`, `SESSION_TRANSCRIPT.md`, and
  `SUMMARY.md`.
- [`docs/testreports/`](./docs/testreports/) — smoke/manual test narratives and screenshots
  referenced above.
- [`docs/usernotes/`](./docs/usernotes/) — raw scratch notes kept during the process, ahead of/around
  the formal prompt history: [`gemini-kickoff-notes.txt`](./docs/usernotes/gemini-kickoff-notes.txt)
  (early folder-structure sketch and brief-interpretation notes from an exploratory session with a
  different model, predating the final `docs/prompts/` convention) and
  [`promptnotes-tmp.txt`](./docs/usernotes/promptnotes-tmp.txt) (the model used per phase, and the
  running log of prompts actually sent to the CLI agent across all 5 phases — the closest thing to a
  raw transcript of the human side of the conversation).
