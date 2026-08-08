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

## FI-002 (placeholder for future entries)

Add new entries here as `FI-00N` following the same Context / Current shortcut / Proper fix / Why
deferred structure.
