using DailyBugle.Domain.Enums;
using DailyBugle.Domain.Exceptions;
using DailyBugle.Notifications;

namespace DailyBugle.Engine;

/// <summary>
/// Strategy selector resolving a <see cref="ChannelType"/> to its registered <see cref="INotificationChannel"/>
/// implementation. Adding a new channel (e.g. Microsoft Teams) only requires implementing
/// <see cref="INotificationChannel"/> and registering it here/in DI — it never requires changes to
/// <see cref="AlertDispatcher"/> (see ARCHITECTURE.md &#167;6, extensibility).
/// </summary>
public sealed class NotificationChannelResolver
{
    private readonly IReadOnlyDictionary<ChannelType, INotificationChannel> _channelsByType;

    /// <summary>
    /// Creates a resolver from the set of registered channel implementations, keyed by each
    /// implementation's own <see cref="INotificationChannel.Channel"/> value.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="channels"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when two channels register the same <see cref="ChannelType"/>.</exception>
    public NotificationChannelResolver(IEnumerable<INotificationChannel> channels)
    {
        ArgumentNullException.ThrowIfNull(channels);
        _channelsByType = channels.ToDictionary(c => c.Channel);
    }

    /// <summary>
    /// Resolves the <see cref="INotificationChannel"/> registered for <paramref name="channelType"/>.
    /// </summary>
    /// <exception cref="InvalidNotificationChannelException">Thrown when no implementation is registered for <paramref name="channelType"/>.</exception>
    public INotificationChannel Resolve(ChannelType channelType) =>
        _channelsByType.TryGetValue(channelType, out var channel)
            ? channel
            : throw new InvalidNotificationChannelException(channelType);
}
