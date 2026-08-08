# DailyBugle.SecretsTool

Standalone console tool that encrypts local plaintext credentials (Gmail App Password, Slack
Incoming Webhook URL) into `secrets.local.json`, consumed by `DailyBugle.Wpf` (and optionally
`DailyBugle.SmokeTest`) at startup. See
[`docs/DECISION_LOG.md` D-009/D-009a](../../docs/DECISION_LOG.md#d-009-secrets-handling--real-credentials-never-committed)
for why this exists: plaintext credentials must never be committed **or** stored locally, even in a
gitignored file.

## What it does

1. Reads a plaintext input file of `key=value` lines you provide (blank lines and `#` comments are
   skipped).
2. Validates the three required keys are present and non-empty:
   - `GMAIL_SENDER_EMAIL`
   - `GMAIL_APP_PASSWORD`
   - `SLACK_WEBHOOK_URL`
3. Encrypts them via `SecretProtector` (Windows DPAPI, `DataProtectionScope.CurrentUser` — tied to
   the Windows account running the tool) and writes the ciphertext to `secrets.local.json`.
4. Deletes the plaintext input file after a successful write, so no plaintext copy of your
   credentials lingers on disk.
5. Never prints any secret value to the console — only status/paths and, on failure, which keys
   were missing (not their values).

## Prerequisites

- Windows (DPAPI is Windows-only; this is why the whole solution targets Windows/WPF).
- A Gmail account with **2-Step Verification enabled** and a generated
  [App Password](https://myaccount.google.com/apppasswords) — a normal account password will fail
  SMTP auth (see
  [`docs/testreports/phase2-smoke-test-attempt1-gmail-auth-failed.md`](../../docs/testreports/phase2-smoke-test-attempt1-gmail-auth-failed.md)
  for the exact failure mode this avoids).
- A Slack Incoming Webhook URL for the channel you want notifications posted to.

## Usage

1. Create a plaintext file (anywhere; a temp location is fine — it gets deleted after a successful
   run), e.g. `secrets-input.txt`:
   ```
   GMAIL_SENDER_EMAIL=you@gmail.com
   GMAIL_APP_PASSWORD=xxxxxxxxxxxxxxxx
   SLACK_WEBHOOK_URL=https://hooks.slack.com/services/...
   ```
2. Run the tool and follow its interactive prompts:
   ```powershell
   cd src\DailyBugle.SecretsTool
   dotnet run
   ```
   - When asked for the input file path, provide the path from step 1.
   - When asked for the output path, press Enter to accept the default
     (`src/DailyBugle.Wpf/secrets.local.json`) unless you have a specific reason to change it —
     `DailyBugle.Wpf` and `DailyBugle.SmokeTest` both expect it there.
3. Re-run this tool any time you rotate the Gmail App Password or the Slack webhook URL — it
   overwrites the existing `secrets.local.json`.

## Why a separate tool instead of a config UI

Keeps credential handling out of the main `DailyBugle.Wpf` app entirely — the WPF app only ever
*reads* an already-encrypted file (`EncryptedSecretsStore.Load`) and fails fast if it's
missing/invalid; it never accepts or displays plaintext secrets itself. This tool is the only place
plaintext credentials are typed in, and it minimizes their lifetime on disk (auto-deleted after
encryption).

## Related

- [`docs/ARCHITECTURE.md` §8 — Secrets & Configuration](../../docs/ARCHITECTURE.md#8-secrets--configuration)
- [`docs/DECISION_LOG.md` D-009a](../../docs/DECISION_LOG.md#d-009a-secrets-storage-hardened--encrypted-at-rest-not-just-gitignored)
- [`src/DailyBugle.SmokeTest/README.md`](../DailyBugle.SmokeTest/README.md) — the other consumer of
  `secrets.local.json`
