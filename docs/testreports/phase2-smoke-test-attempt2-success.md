# Phase 2 — Manual Smoke Test Report (Attempt 2 — after Gmail App Password fix)

> Companion docs: [PLAN.md](../PLAN.md) · [ARCHITECTURE.md](../ARCHITECTURE.md) ·
> [DECISION_LOG.md](../DECISION_LOG.md) · [FUTURE_IMPROVEMENTS.md](../FUTURE_IMPROVEMENTS.md) ·
> [phase2-smoke-test-attempt1-gmail-auth-failed.md](./phase2-smoke-test-attempt1-gmail-auth-failed.md)
> (previous attempt — Gmail SMTP auth failure, kept for history)

**Purpose:** Re-validate Phase 2's acceptance criterion (`PLAN.md` §6) after the user enabled
2-Step Verification and generated a fresh Gmail App Password (root cause of Attempt 1's failure).

**Date/time:** 2026-08-08, ~13:50.

## What changed since Attempt 1

- User enabled 2-Step Verification on the Gmail account and generated a new App Password at
  `myaccount.google.com/apppasswords`.
- New plaintext credentials placed in `inputdata-notencrypted.txt`; re-run through
  `DailyBugle.SecretsTool` to re-encrypt `secrets.local.json` (old ciphertext overwritten,
  plaintext file auto-deleted again after the run, per its documented behavior).
- No production code changes — same `EmailNotificationChannel`/`SlackNotificationChannel`/
  `AlertDispatcher` implementation as Attempt 1, confirming the Attempt 1 failure was indeed
  credential/account-side, not a code defect.

## Harness

Same harness approach as Attempt 1: a local, non-committed console program
(`DailyBugle.SmokeTest`, kept on disk as a reusable dev tool but **not** added to `DailyBugle.sln`
and **not** wired into the WPF app) manually wires the real backend components with no mocks —
real `SecretProtector`/`EncryptedSecretsStore`, real `DemoDataSeeder`, real
`EmailNotificationChannel` (Gmail SMTP) and `SlackNotificationChannel` (Slack webhook), real
`NewsSimulator` → `AlertDispatcher` → `NotificationChannelResolver` flow. Two events fired: a
`Science` event (multi-user, multi-channel) and a `Sport` control event (single-recipient).

## Results

### Science event (multi-user, multi-channel)

| User | Channel | Success | Notes |
|---|---|---|---|
| Estebán Alemán | Slack | ✅ **True** | Real webhook POST delivered successfully |
| Németh István | Email | ✅ **True** | Real Gmail SMTP send succeeded (App Password now valid) |

### Sport event (single-recipient control)

| User | Channel | Success | Notes |
|---|---|---|---|
| Estebán Alemán | — | — | **No delivery record** — correctly excluded (no Sport rule) |
| Németh István | Email | ✅ **True** | Real Gmail SMTP send succeeded |

## Analysis

**All Phase 2 acceptance criteria now confirmed live, with no mocks:**
- `AlertDispatcher` rule-matching: `Science` event correctly dispatched to **both** users across
  **two different channels** in one cycle (D-014's target test case); `Sport` control event
  correctly matched **only** Németh's rule, zero records for Estebán.
- `NotificationChannelResolver` Strategy selection: both channel types resolved correctly.
- `SlackNotificationChannel`: **validated end-to-end** (also confirmed in Attempt 1).
- `EmailNotificationChannel`: **validated end-to-end** — real Gmail SMTP send succeeded for both
  the Science and Sport events.
- `DeliveryRecord` logging: correct success/failure/absence semantics across both events.
- `SecretProtector`/`EncryptedSecretsStore` round-trip: confirmed twice now (Attempt 1's
  decrypt-then-fail-at-Gmail, and this attempt's decrypt-then-succeed), proving the encryption
  layer itself was never the issue — Attempt 1's root cause was purely the missing 2FA/App
  Password on the Google account, exactly as hypothesized.

## Status

- ✅ Multi-user/multi-channel dispatch logic: **validated**
- ✅ Slack live delivery: **validated**
- ✅ Gmail live delivery: **validated** — Attempt 1's root cause (no 2FA / no valid App Password)
  resolved by the user; no code changes were required.

**Phase 2 acceptance criteria (`PLAN.md` §6) are now fully met.**
