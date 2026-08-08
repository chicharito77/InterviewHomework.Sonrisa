# DailyBugle — Project Plan

> Companion docs: [ARCHITECTURE.md](./ARCHITECTURE.md) · [DECISION_LOG.md](./DECISION_LOG.md)

## 1. Context

Brief (verbatim in [`task.txt`](./task.txt)): build a flexible alerting system that notifies users about
important world events (breaking news, market movements, natural disasters, ...) via Email and Slack,
extensible to future channels, with an admin view. No further spec provided — ambiguity is intentional
and part of the evaluation.

**Time budget:** 6 hours total (interview homework — see `.github/copilot-instructions.md`).
**Evaluation focus:** how the brief was scoped and executed, not a polished final product.

## 2. Phases & Deliverables

| # | Phase | Deliverables | Status |
|---|-------|--------------|--------|
| 1 | Kickoff & Planning / Architecture | `docs/PLAN.md`, `docs/ARCHITECTURE.md`, `docs/DECISION_LOG.md` | ✅ In progress (this doc) |
| 2 | Backend implementation | `DailyBugle.sln` under `src/`; Domain (`Event`, `AlertRule`, `User`, `DateTimeHandler`), Channels (`INotificationChannel`, `EmailNotificationChannel`, `SlackNotificationChannel`), Engine (`AlertDispatcher`, `NewsSimulator`), in-memory repositories | ⬜ Not started |
| 3 | Frontend implementation | WPF app (MVVM), 2 tabs: **User** (rules list, add rule, notification history), **Admin** (users list, fire event) | ⬜ Not started |
| 4 | Tests | ≥3 NUnit/Moq fixtures covering core logic (`AlertDispatcher` matching/dispatch, `AlertRule` behavior, `INotificationChannel` strategy resolution) | ⬜ Not started |
| 5 | Documentation pass | README refresh, cross-links between md files, consistent English docs, final repo scan | ⬜ Not started |

## 3. Scope Decisions (see DECISION_LOG.md for full rationale)

**In scope:**
- Domain: `Event` (NewsType: Sport/Economy/Science), `AlertRule` (User + NewsType + Channel), `User` (owns multiple `AlertRule`s)
- Channels: real **Gmail SMTP** email delivery, real **Slack Incoming Webhook** delivery
- Engine: `NewsSimulator` (manual, single-shot event firing), `AlertDispatcher` (Observer/Pub-Sub — matches events to rules, dispatches via Strategy-selected channel)
- Persistence: in-memory, thread-safe repositories (no database)
- Two pre-seeded demo users — Németh István (Email; Sport **and** Science) and Estebán Alemán (Slack; Science **and** Economy), sharing a Science interest so a fired Science event can be verified to notify both users independently — **no registration/user-creation UI**; users and their initial rules are seeded in code (see ARCHITECTURE.md §9)
- Secrets (Gmail App Password, Slack webhook URL, seed contact addresses) encrypted at rest locally (DPAPI), never committed (DECISION_LOG.md D-009/D-009a)
- WPF UI: global "Acting as" identity switcher (Admin + 2 seeded users) toggling exclusive tab visibility, Admin tab (users list + fire-event controls), User tab (rule list + add rule + per-user notification history panel) — see ARCHITECTURE.md §10
- Testability: `DateTimeHandler` abstraction, SOLID/DI throughout, ≥3 test fixtures

**Explicitly out of scope (Phase 2 / backlog, documented not built):**
- Microsoft Teams channel (documented extension point only)
- Automatic/periodic event generation (timer-based simulation)
- AlertRule filtering by severity/keyword (news-type-only matching for now)
- Admin notification history / delivery monitoring log (User tab has its own per-user history instead, see D-012)
- User registration / account-creation UI (users are code-seeded, see above)
- Persistent (non-in-memory) storage
- Authentication / multi-tenant user login

## 4. Execution Order & Dependencies

```
Phase 1 (Planning) ──▶ Phase 2 (Backend) ──▶ Phase 3 (Frontend) ──▶ Phase 4 (Tests) ──▶ Phase 5 (Docs)
                                  │                                        ▲
                                  └──── Phase 4 can start on Domain/Engine ─┘
                                        as soon as it stabilizes (parallelizable
                                        with tail end of Phase 3 if time allows)
```

Rationale: backend (domain + engine + channels) must exist before UI can bind to it. Tests target
backend logic primarily, so they can start once Phase 2 stabilizes rather than strictly waiting for
Phase 3 — but given the 6h budget, phases will mostly run sequentially with tests written
alongside/after backend classes are finalized.

## 5. Time Allocation (revised — hard deadline today 15:30)

Session start ≈ 10:31, current time ≈ 12:07 → **~3h23m remaining** until deadline (refreshed after
Phase 1 review/clarification rounds ran longer than the original estimate). Remaining phases
rescaled again from the prior revision:

| Phase | Original Est. | 1st Revision (@11:39) | Current Revision (@12:07) |
|-------|---------------|------------------------|------------------------------|
| 1. Planning & Architecture | ~1.0h | ~1h10m | ~1h35m (actual, now closing out) |
| 2. Backend implementation | ~2.25h | ~1h45m | **~1h32m** |
| 3. Frontend implementation | ~1.5h | ~1h10m | **~1h02m** |
| 4. Tests | ~0.75h | ~35m | **~31m** |
| 5. Documentation pass | ~0.5h | ~20m | **~18m** |

**Risk called out:** the budget keeps compressing as planning takes longer than estimated — this is
expected given the brief's deliberate ambiguity, but it means Phase 2 must start immediately with no
further scope discussion. If time runs short, cut in this order (last-defined = first cut): Phase 5
polish → Phase 4 down to exactly 3 fixtures → Phase 3 "Add New Rule" convenience UI (keep list +
fire-event + history as non-negotiable) → Phase 2 stays intact (everything else depends on it).

## 6. Acceptance Criteria (per phase)

- **Phase 2:** Solution builds; `NewsSimulator.Publish(...)` → `AlertDispatcher` correctly resolves matching `AlertRule`s → correct `INotificationChannel` invoked; a fired event for a subscribed user actually sends a real Slack message / Gmail email in manual smoke test.
- **Phase 3:** WPF app launches; Admin tab can list users and fire an event; User tab can list and add rules; UI reflects changes without restart.
- **Phase 4:** `dotnet test` passes; ≥3 fixtures around `AlertDispatcher`/`AlertRule`/channel resolution using Moq for `INotificationChannel` and `IDateTimeProvider`.
- **Phase 5:** README documents setup/run steps and domain concepts; ARCHITECTURE.md/DECISION_LOG.md/PLAN.md cross-linked; no secrets committed.

## 7. Risks / Assumptions Carried Forward

- Real email/Slack delivery depends on user-supplied secrets (Gmail App Password, Slack webhook) — encrypted at rest locally (DPAPI, see DECISION_LOG.md D-009a), never committed; feature must degrade gracefully (fail-fast + logged error) if secrets are absent.
- Deadline-compressed budget (~3h51m for Phases 2–5, see §5) is tight for real external integrations + WPF + tests — scope deliberately minimized (no severity filters, no Teams impl, no history log, no registration UI) to protect this timeline.
- Two fixed seed users for the POC demo; not load-tested, not representative of multi-tenant scale.
- `DeliveryRecord` history is per-user (User tab) only — no cross-user admin monitoring log (D-008/D-012).
