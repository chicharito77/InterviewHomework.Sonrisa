using DailyBugle.Domain.Enums;

namespace DailyBugle.Domain.Entities;

/// <summary>
/// A single world event published by the news feed (real or simulated via <c>NewsSimulator</c>),
/// categorized by <see cref="NewsType"/> so it can be matched against subscribers' <see cref="AlertRule"/>s.
/// </summary>
public sealed class Event
{
    /// <summary>Unique identifier of the event.</summary>
    public Guid Id { get; }

    /// <summary>Category of this event.</summary>
    public NewsType NewsType { get; }

    /// <summary>Short headline describing the event.</summary>
    public string Title { get; }

    /// <summary>Longer description/body of the event.</summary>
    public string Description { get; }

    /// <summary>Timestamp the event occurred/was published, via <see cref="Abstractions.IDateTimeProvider"/>.</summary>
    public DateTime OccurredAt { get; }

    /// <summary>
    /// Creates a new <see cref="Event"/>.
    /// </summary>
    /// <param name="newsType">Category of the event.</param>
    /// <param name="title">Short headline. Must not be null or empty.</param>
    /// <param name="description">Longer description. Must not be null or empty.</param>
    /// <param name="occurredAt">Timestamp the event occurred (from <see cref="Abstractions.IDateTimeProvider"/>).</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="title"/> or <paramref name="description"/> is null or empty.</exception>
    public Event(NewsType newsType, string title, string description, DateTime occurredAt)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title must not be null or empty.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description must not be null or empty.", nameof(description));
        }

        Id = Guid.NewGuid();
        NewsType = newsType;
        Title = title;
        Description = description;
        OccurredAt = occurredAt;
    }
}
