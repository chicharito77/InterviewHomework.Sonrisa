using DailyBugle.Domain.Enums;

namespace DailyBugle.Domain.Entities;

/// <summary>
/// Records the outcome of a single dispatch attempt (one <see cref="AlertRule"/> match against one
/// <see cref="Event"/>). Backs the User tab's per-user notification history panel (see DECISION_LOG.md D-012).
/// </summary>
public sealed class DeliveryRecord
{
    /// <summary>Unique identifier of this delivery record.</summary>
    public Guid Id { get; }

    /// <summary>Identifier of the <see cref="Event"/> that triggered this delivery attempt.</summary>
    public Guid EventId { get; }

    /// <summary>Identifier of the <see cref="User"/> the notification was addressed to.</summary>
    public Guid UserId { get; }

    /// <summary>Channel the notification was attempted through.</summary>
    public ChannelType Channel { get; }

    /// <summary>Timestamp the delivery attempt occurred, via <see cref="Abstractions.IDateTimeProvider"/>.</summary>
    public DateTime OccurredAt { get; }

    /// <summary>Whether the delivery attempt succeeded.</summary>
    public bool Success { get; }

    /// <summary>Error detail when <see cref="Success"/> is false; null when the delivery succeeded.</summary>
    public string? ErrorMessage { get; }

    private DeliveryRecord(Guid eventId, Guid userId, ChannelType channel, DateTime occurredAt, bool success, string? errorMessage)
    {
        Id = Guid.NewGuid();
        EventId = eventId;
        UserId = userId;
        Channel = channel;
        OccurredAt = occurredAt;
        Success = success;
        ErrorMessage = errorMessage;
    }

    /// <summary>Creates a record for a successful delivery attempt.</summary>
    public static DeliveryRecord Succeeded(Guid eventId, Guid userId, ChannelType channel, DateTime occurredAt) =>
        new(eventId, userId, channel, occurredAt, success: true, errorMessage: null);

    /// <summary>Creates a record for a failed delivery attempt.</summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errorMessage"/> is null or empty.</exception>
    public static DeliveryRecord Failed(Guid eventId, Guid userId, ChannelType channel, DateTime occurredAt, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException("ErrorMessage must not be null or empty for a failed delivery.", nameof(errorMessage));
        }

        return new DeliveryRecord(eventId, userId, channel, occurredAt, success: false, errorMessage);
    }
}
