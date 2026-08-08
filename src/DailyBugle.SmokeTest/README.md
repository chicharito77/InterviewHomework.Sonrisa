# DailyBugle.SmokeTest

Manual, real-delivery smoke-test harness for the DailyBugle backend (Domain/Notifications/Engine/
Infrastructure). Not part of `DailyBugle.sln` and not referenced by the WPF app — this is a
standalone dev tool kept for repeatable manual verification of the full dispatch pipeline against
real external services (no mocks).

See [`docs/testreports/`](../../docs/testreports/) for reference runs and their results.

## What it does

Wires the real backend components directly (no DI container, no mocks):

1. Loads and decrypts `src/DailyBugle.Wpf/secrets.local.json` via `EncryptedSecretsStore`/`SecretProtector`.
2. Seeds the two demo users (Németh István / Estebán Alemán) and their 4 `AlertRule`s via `DemoDataSeeder`.
3. Wires real `EmailNotificationChannel` (Gmail SMTP) and `SlackNotificationChannel` (Slack webhook)
   behind `NotificationChannelResolver`.
4. Fires a `Science` event (expects both users notified across both channels) and a `Sport` control
   event (expects only Németh notified), then prints the resulting `DeliveryRecord`s.

## Prerequisites

- A valid `src/DailyBugle.Wpf/secrets.local.json` — generate/refresh it by running
  `DailyBugle.SecretsTool` first (see its own usage prompts).
- Gmail account used for `GMAIL_SENDER_EMAIL` must have 2-Step Verification enabled and a valid
  App Password (see `docs/testreports/phase2-smoke-test-attempt1-gmail-auth-failed.md` for the
  failure mode when this isn't set up correctly).

## Usage

```powershell
cd src\DailyBugle.SmokeTest
dotnet run
```

No secret values are ever printed to the console — only generic status/result lines
(`Success=True/False`, channel names, error messages from the delivery attempt itself).

## Why it's not in `DailyBugle.sln`

Kept deliberately separate so `dotnet build`/`dotnet test` on the main solution never require Gmail/
Slack connectivity or valid secrets — this tool is opt-in, run manually when live-delivery
verification is needed (e.g. after changing `EmailNotificationChannel`/`SlackNotificationChannel`,
or after rotating credentials via `DailyBugle.SecretsTool`).
