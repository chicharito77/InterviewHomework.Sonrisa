namespace DailyBugle.Infrastructure.Security;

/// <summary>
/// On-disk shape of <c>secrets.local.json</c>. Every property holds DPAPI ciphertext produced by
/// <see cref="SecretProtector.Protect"/> — never plaintext. This file is gitignored and must never be
/// committed (see DECISION_LOG.md D-009/D-009a).
/// </summary>
public sealed class SecretsFile
{
    /// <summary>Ciphertext for the Gmail sender/authenticating mailbox address.</summary>
    public required string GmailSenderEmailCipher { get; init; }

    /// <summary>Ciphertext for the Gmail App Password.</summary>
    public required string GmailAppPasswordCipher { get; init; }

    /// <summary>Ciphertext for the Slack Incoming Webhook URL.</summary>
    public required string SlackWebhookUrlCipher { get; init; }
}
