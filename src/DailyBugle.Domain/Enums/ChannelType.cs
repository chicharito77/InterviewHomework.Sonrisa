namespace DailyBugle.Domain.Enums;

/// <summary>
/// Identifies a notification delivery mechanism. Selected via the Strategy pattern by
/// <c>NotificationChannelResolver</c> to resolve the matching <c>INotificationChannel</c>.
/// </summary>
public enum ChannelType
{
    /// <summary>Delivery via email (SMTP).</summary>
    Email,

    /// <summary>Delivery via a Slack Incoming Webhook.</summary>
    Slack
}
