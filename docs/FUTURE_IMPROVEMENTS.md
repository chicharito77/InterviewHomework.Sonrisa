# DailyBugle — Future Improvements / Backlog

> Companion docs: [PLAN.md](./PLAN.md) · [ARCHITECTURE.md](./ARCHITECTURE.md) · [DECISION_LOG.md](./DECISION_LOG.md)

Ideas and known shortcuts explicitly deferred past this POC's 6-hour budget, or discovered as
rough edges during implementation. Not committed to any timeline — tracked here so they aren't
forgotten, separate from the already-documented "Out of Scope" list in `PLAN.md` §3.

---

## FI-001: `User.Email` is required, but Estebán Alemán is a Slack-only subscriber

- **Context:** `User.Email` (Domain entity) is a required, non-empty field (see
  `DailyBugle.Domain/Entities/User.cs`), because `EmailNotificationChannel` needs a target address
  for any user with an active Email-channel `AlertRule`. Estebán Alemán, however, is seeded as a
  **Slack-only** subscriber (`DECISION_LOG.md` D-011/D-014) — he has no real personal email address
  supplied to this POC (only one Gmail sender credential was provided, for Németh István's
  self-test email loop and as the app's outgoing SMTP account).
- **Current shortcut:** `DemoDataSeeder` reuses the same Gmail sender address as Estebán's
  `User.Email` placeholder, purely to satisfy the entity's non-empty constructor guard. This value
  is never actually used for delivery today, since Estebán has no Email-channel `AlertRule` —  it
  would only matter if a future rule mistakenly added an Email subscription for him, in which case
  he'd "receive" a copy at Németh's own inbox rather than a real address of his own (not a real
  privacy leak, since it's the same self-test account — but still a data-modeling smell worth
  fixing).
- **Proper fix (future work):** make `User.Email` nullable (`string?`), matching the existing
  optionality already modeled on `User.SlackWebhookUrl`; have `EmailNotificationChannel` throw the
  existing `InvalidOperationException` guard when a user has no email configured (the code path
  already exists for the symmetric Slack case). Would need a small follow-up check in
  `AlertRuleService.AddRule`/UI validation to prevent a UI from letting a user pick a channel they
  have no contact target for in the first place.
- **Why deferred:** low risk within current POC scope (no Email rule is ever seeded for Estebán,
  so the placeholder is inert); fixing it now would touch the entity, the channel's null-guard
  wording, and DI wiring, for no functional gain inside the 6-hour budget.
- **Flagged by:** user review during Phase 2 (Infrastructure/seed data implementation).

---

## FI-002: "Active" checkbox on the User tab's Alert Rules grid is not editable

- **Context:** `Views/UserView.xaml`'s Alert Rules `DataGrid` shows an `Active`
  `DataGridCheckBoxColumn` bound to `RuleRowViewModel.IsActive`, giving the visual impression a user
  can temporarily deactivate a rule without deleting it.
- **Current shortcut:** the whole `DataGrid` is declared `IsReadOnly="True"`, which makes every
  column non-interactive, including the checkbox. There is also no command/service method wired up
  yet to persist an `IsActive` toggle back through `AlertRuleService`/`IAlertRuleRepository` even if
  the grid were made editable — `AlertRuleService` currently only exposes `AddRule`/`RemoveRule`, no
  `SetActive`/`ToggleActive`.
- **Proper fix (future work):** add `AlertRuleService.SetActive(Guid ruleId, bool isActive)` (mutating
  the in-memory repository), remove `IsReadOnly` from the checkbox column only (or the whole grid,
  keeping other columns read-only via per-column `IsReadOnly="True"`), and wire the checkbox's
  two-way binding to call the new service method on change (e.g. via a small `[RelayCommand]` bound
  to the checkbox's `Checked`/`Unchecked` events, since `DataGridCheckBoxColumn` doesn't expose a
  `Command` directly).
- **Why deferred:** current workaround (delete + re-add the rule) covers the same functional need
  within the 6-hour POC budget; not part of the Phase 3 acceptance criterion in `PLAN.md` §6.
- **Flagged by:** user manual testing, Phase 3 Round 2 (`docs/testreports/phase3-frontend-manual-test-round2.md`, TC4).

---

## FI-003: Admin tab's "Registered Users" list does not live-update when a user's rules change

- **Context:** the Admin tab's `Users` list (`AdminViewModel.Users`) shows each registered user's
  active channels/news-type summary, rebuilt from `AlertRuleService.GetRulesForUser`.
- **Current shortcut:** `AdminViewModel.RefreshUsers()` is only ever invoked once, from the
  `AdminViewModel` constructor at app startup. `UserViewModel.AddRule`/`RemoveRule` (User tab) only
  call their own local `RefreshRules()`, and have no reference to `AdminViewModel` nor raise any
  shared event `AdminViewModel` could subscribe to — so a rule added/removed on the User tab is
  invisible on the Admin tab's user summary until the app is restarted.
- **Proper fix (future work):** introduce a small shared pub/sub notification (e.g. an
  `IAlertRuleChangeNotifier`/event aggregator singleton, or have `AlertRuleService.AddRule`/
  `RemoveRule` raise a `RulesChanged` event) that both `UserViewModel` and `AdminViewModel` subscribe
  to, so `AdminViewModel.RefreshUsers()` re-runs whenever any user's rule set changes — mirroring the
  existing `AlertDispatcher.DispatchCompleted` reactive pattern already used for notification
  history.
- **Why deferred:** cosmetic/consistency issue only (switching tabs and using the app normally still
  works; the underlying repository data is correct, only the Admin summary view is stale); adding a
  cross-ViewModel event bus was judged lower priority than the Phase 3 acceptance criteria within the
  6-hour POC budget.
- **Flagged by:** user manual testing, Phase 3 Round 2 (`docs/testreports/phase3-frontend-manual-test-round2.md`, TC1 & TC4).

---

## FI-004 (placeholder for future entries)

Add new entries here as `FI-00N` following the same Context / Current shortcut / Proper fix / Why
deferred structure.
