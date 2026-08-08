using DailyBugle.Domain.Enums;

namespace DailyBugle.Domain.Entities;

/// <summary>
/// A user's subscription to a <see cref="Enums.NewsType"/> via a specific <see cref="Enums.ChannelType"/>.
/// Matched against published <see cref="Event"/>s by the dispatcher (Observer pattern).
/// </summary>
public sealed class AlertRule
{
    /// <summary>Unique identifier of the rule.</summary>
    public Guid Id { get; }

    /// <summary>Owning <see cref="User"/>'s identifier.</summary>
    public Guid UserId { get; }

    /// <summary>News category this rule subscribes to.</summary>
    public NewsType NewsType { get; }

    /// <summary>Delivery channel this rule uses.</summary>
    public ChannelType Channel { get; }

    /// <summary>Whether this rule is currently active and eligible for matching.</summary>
    public bool IsActive { get; private set; }

    /// <summary>Timestamp the rule was created, via <see cref="Abstractions.IDateTimeProvider"/>.</summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// Creates a new, active <see cref="AlertRule"/>.
    /// </summary>
    /// <param name="userId">Owning user's identifier. Must not be empty.</param>
    /// <param name="newsType">Subscribed news category.</param>
    /// <param name="channel">Delivery channel.</param>
    /// <param name="createdAt">Creation timestamp (from <see cref="Abstractions.IDateTimeProvider"/>).</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="userId"/> is empty.</exception>
    public AlertRule(Guid userId, NewsType newsType, ChannelType channel, DateTime createdAt)
        : this(Guid.NewGuid(), userId, newsType, channel, createdAt)
    {
    }

    /// <summary>
    /// Creates an active <see cref="AlertRule"/> with an explicit <paramref name="id"/> (e.g. for deterministic seed data).
    /// </summary>
    public AlertRule(Guid id, Guid userId, NewsType newsType, ChannelType channel, DateTime createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId must not be empty.", nameof(userId));
        }

        Id = id;
        UserId = userId;
        NewsType = newsType;
        Channel = channel;
        CreatedAt = createdAt;
        IsActive = true;
    }

    /// <summary>Marks the rule as inactive, excluding it from future dispatch matching.</summary>
    public void Deactivate() => IsActive = false;

    /// <summary>Returns true if this active rule subscribes to <paramref name="newsType"/>.</summary>
    public bool Matches(NewsType newsType) => IsActive && NewsType == newsType;
}
