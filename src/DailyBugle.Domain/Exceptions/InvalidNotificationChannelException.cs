using DailyBugle.Domain.Enums;

namespace DailyBugle.Domain.Exceptions;

/// <summary>
/// Thrown by <c>NotificationChannelResolver</c> when no <c>INotificationChannel</c> implementation
/// is registered for the requested <see cref="ChannelType"/>.
/// </summary>
public sealed class InvalidNotificationChannelException : Exception
{
    /// <summary>The channel type that could not be resolved to a registered implementation.</summary>
    public ChannelType Channel { get; }

    /// <summary>Creates a new <see cref="InvalidNotificationChannelException"/> for the given <paramref name="channel"/>.</summary>
    public InvalidNotificationChannelException(ChannelType channel)
        : base($"No notification channel implementation is registered for channel type '{channel}'.")
    {
        Channel = channel;
    }
}
