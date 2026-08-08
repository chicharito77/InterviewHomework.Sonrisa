using DailyBugle.Domain.Enums;

namespace DailyBugle.Domain.Entities;

/// <summary>
/// A subscriber who owns zero or more <see cref="AlertRule"/>s. Represents a real recipient with a
/// resolvable contact target for at least one <see cref="ChannelType"/>.
/// </summary>
public sealed class User
{
    /// <summary>Unique identifier of the user.</summary>
    public Guid Id { get; }

    /// <summary>Display name of the user.</summary>
    public string Name { get; }

    /// <summary>Email address used as the target for <c>EmailNotificationChannel</c> deliveries.</summary>
    public string Email { get; }

    /// <summary>
    /// Slack Incoming Webhook URL used as the target for <c>SlackNotificationChannel</c> deliveries.
    /// Optional: not every user is subscribed to Slack-channel alerts.
    /// </summary>
    public string? SlackWebhookUrl { get; }

    /// <summary>
    /// Creates a new <see cref="User"/>.
    /// </summary>
    /// <param name="name">Display name. Must not be null or empty.</param>
    /// <param name="email">Contact email. Must not be null or empty.</param>
    /// <param name="slackWebhookUrl">Optional Slack Incoming Webhook URL.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> or <paramref name="email"/> is null or empty.</exception>
    public User(string name, string email, string? slackWebhookUrl = null)
        : this(Guid.NewGuid(), name, email, slackWebhookUrl)
    {
    }

    /// <summary>
    /// Creates a <see cref="User"/> with an explicit <paramref name="id"/> (e.g. for deterministic seed data).
    /// </summary>
    public User(Guid id, string name, string email, string? slackWebhookUrl = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name must not be null or empty.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email must not be null or empty.", nameof(email));
        }

        Id = id;
        Name = name;
        Email = email;
        SlackWebhookUrl = slackWebhookUrl;
    }
}
