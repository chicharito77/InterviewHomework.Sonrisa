using DailyBugle.Domain.Entities;
using DailyBugle.Domain.Enums;

namespace DailyBugle.Notifications;

/// <summary>
/// Strategy interface for delivering a notification about a published <see cref="Event"/> to a
/// specific <see cref="User"/> through one interchangeable delivery mechanism (email, Slack, ...).
/// Selected at runtime by <c>NotificationChannelResolver</c> based on <see cref="Channel"/>.
/// </summary>
public interface INotificationChannel
{
    /// <summary>The channel type this implementation handles.</summary>
    ChannelType Channel { get; }

    /// <summary>
    /// Sends a notification about <paramref name="event"/> to <paramref name="user"/>.
    /// </summary>
    /// <param name="user">Recipient. Must have a valid contact target for this channel.</param>
    /// <param name="event">The published event to notify about.</param>
    /// <param name="cancellationToken">Token to cancel the outbound delivery.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="user"/> or <paramref name="event"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="user"/> lacks a valid contact target for this channel.</exception>
    Task SendAsync(User user, Event @event, CancellationToken cancellationToken = default);
}
