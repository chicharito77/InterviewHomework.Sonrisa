namespace DailyBugle.Notifications.Configuration;

/// <summary>
/// Non-secret shape for outgoing SMTP configuration. Values (in particular <see cref="SenderEmail"/>
/// and <see cref="SenderPassword"/>) are populated at startup from the encrypted local secret store
/// (see DECISION_LOG.md D-009a) — never hardcoded in source or committed configuration.
/// </summary>
public sealed class EmailChannelOptions
{
    /// <summary>SMTP server host (e.g. <c>smtp.gmail.com</c>).</summary>
    public required string SmtpHost { get; init; }

    /// <summary>SMTP server port (e.g. <c>587</c> for STARTTLS).</summary>
    public required int SmtpPort { get; init; }

    /// <summary>Sender/authenticating mailbox address.</summary>
    public required string SenderEmail { get; init; }

    /// <summary>Sender mailbox password (Gmail App Password), decrypted at startup — never logged or persisted in plaintext.</summary>
    public required string SenderPassword { get; init; }

    /// <summary>Whether to use SSL/TLS for the SMTP connection. Defaults to true.</summary>
    public bool UseSsl { get; init; } = true;

    /// <summary>Send timeout, in seconds. Defaults to 15.</summary>
    public int TimeoutSeconds { get; init; } = 15;
}
