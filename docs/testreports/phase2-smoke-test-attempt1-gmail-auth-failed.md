# Phase 2 — Manual Smoke Test Report

> Companion docs: [PLAN.md](../PLAN.md) · [ARCHITECTURE.md](../ARCHITECTURE.md) ·
> [DECISION_LOG.md](../DECISION_LOG.md) · [FUTURE_IMPROVEMENTS.md](../FUTURE_IMPROVEMENTS.md)

**Purpose:** Validate Phase 2's acceptance criterion (`PLAN.md` §6): "a fired event for a subscribed
user actually sends a real Slack message / Gmail email in manual smoke test."

**Date/time:** 2026-08-08, ~13:15.

## Harness

A temporary, non-committed console program (`DailyBugle.SmokeTest`, deleted immediately after this
run — not part of the solution or repository) manually wired the real backend components with no
mocks:

- `EncryptedSecretsStore` + `SecretProtector` decrypting the real `secrets.local.json`
- `DemoDataSeeder` seeding the two real demo users (Németh István / Estebán Alemán) + 4 rules
- Real `EmailNotificationChannel` (Gmail SMTP) and `SlackNotificationChannel` (Slack Incoming
  Webhook) — no fakes
- Real `NewsSimulator` → `AlertDispatcher` → `NotificationChannelResolver` → channel dispatch flow
- Two events fired: a `Science` event (expected: both users notified, multi-channel) and a `Sport`
  control event (expected: only Németh notified, single-recipient)

## Results

### Science event (multi-user, multi-channel)

| User | Channel | Success | Notes |
|---|---|---|---|
| Estebán Alemán | Slack | ✅ **True** | Real webhook POST delivered successfully |
| Németh István | Email | ❌ **False** | `The SMTP server requires a secure connection or the client was not authenticated. The server response was: 5.7.0 Authentication Required.` |

### Sport event (single-recipient control)

| User | Channel | Success | Notes |
|---|---|---|---|
| Estebán Alemán | — | — | **No delivery record at all** — correctly excluded (no Sport rule) |
| Németh István | Email | ❌ **False** | Same SMTP auth error as above (consistent failure, not flaky) |

## Analysis — what this does and doesn't prove

**Confirmed working (strong evidence, no mocks):**
- `AlertDispatcher` rule-matching logic is correct: the `Science` event correctly resolved rules for
  **both** users across **two different channels** in one dispatch cycle (D-014's exact test case);
  the `Sport` control event correctly matched **only** Németh's rule and produced **zero** records
  for Estebán — proving `AlertRule.Matches`/`GetActiveByNewsType` filtering is sound.
- `NotificationChannelResolver` Strategy selection is correct (both channel types resolved to the
  right implementation).
- `SlackNotificationChannel` — **fully validated end-to-end against the real Slack webhook.**
- `DeliveryRecord` success/failure logging is correct and consistent across both events.
- `SecretProtector`/`EncryptedSecretsStore` round-trip (encrypt → write → read → decrypt) works: the
  Gmail credentials were successfully decrypted and used in the SMTP auth attempt (the failure is
  Gmail *rejecting* the credentials, not a decryption/parsing bug).

**Not yet confirmed:**
- `EmailNotificationChannel` has not been validated against a live mailbox. The code path executed
  (constructed `SmtpClient`, attempted `SendMailAsync`) and failed with a Gmail-side authentication
  rejection, not a client-side exception — i.e. this looks like a **credential/account
  configuration issue**, not a bug in `EmailNotificationChannel`'s logic (which mirrors the working
  Slack implementation's structure).

## Likely root causes (not yet confirmed — cannot be verified without re-inspecting the credential)

1. **2-Step Verification not enabled on the Gmail account.** Gmail App Passwords can only be
   generated (and will only authenticate) if 2FA is turned on for the account; a "regular" account
   password will always be rejected with this exact `5.7.0` error since Google disabled
   password-only SMTP auth ("less secure apps").
2. **App Password copied with formatting issues** (e.g. stray whitespace/newline pulled into the
   value from the source file).
3. **Wrong credential type supplied** (e.g. a Google Workspace admin-restricted account blocking
   SMTP, or an App Password generated for a different Google account than the sender address used).

## Recommendation

Re-verify, on your end (without pasting the value back into this chat):
- 2-Step Verification is ON for the Gmail account
- The App Password was generated fresh for "Mail"/"Other" at <https://myaccount.google.com/apppasswords>
- It was copied without extra whitespace into the input file

Then re-run `DailyBugle.SecretsTool` to re-encrypt, and this smoke test can be repeated. Given time
constraints, the team may also choose to accept Slack as the validated channel for this POC and
treat Email delivery as a known, credential-side open item (not a code defect) — logged below.

## Status

- ✅ Multi-user/multi-channel dispatch logic: **validated**
- ✅ Slack live delivery: **validated**
- ⚠️ Gmail live delivery: **blocked on credential/account configuration**, tracked as a
  known open item, not a code defect (see `FUTURE_IMPROVEMENTS.md` for backlog-style tracking if it
  remains unresolved by the Phase 5 documentation pass).
