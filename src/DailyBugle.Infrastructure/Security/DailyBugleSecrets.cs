namespace DailyBugle.Infrastructure.Security;

/// <summary>
/// Decrypted runtime secrets, resolved from <c>secrets.local.json</c> via <see cref="SecretProtector"/>
/// at application startup. Instances only ever hold plaintext transiently in memory — never logged,
/// serialized, or written back to disk in plaintext form (see DECISION_LOG.md D-009a).
/// </summary>
/// <param name="GmailSenderEmail">Gmail sender/authenticating mailbox address (also Németh István's seeded recipient address — self-test loop).</param>
/// <param name="GmailAppPassword">Gmail App Password used for SMTP authentication.</param>
/// <param name="SlackWebhookUrl">Slack Incoming Webhook URL for Estebán Alemán's seeded rules.</param>
public sealed record DailyBugleSecrets(string GmailSenderEmail, string GmailAppPassword, string SlackWebhookUrl);
