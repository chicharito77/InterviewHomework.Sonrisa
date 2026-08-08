namespace DailyBugle.Notifications.Configuration;

/// <summary>
/// Non-secret presentation configuration for outgoing Slack Incoming Webhook messages. The webhook
/// URL itself is per-user (<c>User.SlackWebhookUrl</c>, resolved from the encrypted secret store at
/// startup — see DECISION_LOG.md D-009a) since each subscriber may target a different Slack
/// workspace/channel; this options object only controls how posted messages present themselves.
/// </summary>
public sealed class SlackChannelOptions
{
    /// <summary>Display name shown for messages posted by the bot. Defaults to "DailyBugle".</summary>
    public string BotUsername { get; init; } = " J. Jonah Jameson";

    /// <summary>Send timeout, in seconds. Defaults to 10.</summary>
    public int TimeoutSeconds { get; init; } = 15;
}
