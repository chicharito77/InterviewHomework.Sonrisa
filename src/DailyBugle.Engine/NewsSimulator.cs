using DailyBugle.Domain.Abstractions;
using DailyBugle.Domain.Entities;
using DailyBugle.Domain.Enums;

namespace DailyBugle.Engine;

/// <summary>
/// Manual, single-shot news feed simulator (Observable/Pub-Sub publisher — see DECISION_LOG.md D-005).
/// The Admin tab triggers exactly one <see cref="Event"/> per call to <see cref="Publish"/>; this
/// class has no knowledge of its subscribers (e.g. <see cref="AlertDispatcher"/>).
/// </summary>
public sealed class NewsSimulator : IEventPublisher
{
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <inheritdoc />
    public event EventHandler<Event>? EventPublished;

    /// <summary>Creates a new <see cref="NewsSimulator"/>.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dateTimeProvider"/> is null.</exception>
    public NewsSimulator(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    /// <summary>
    /// Publishes a single new <see cref="Event"/>, raising <see cref="EventPublished"/> for any
    /// subscribers.
    /// </summary>
    /// <param name="newsType">Category of the simulated event.</param>
    /// <param name="title">Short headline. Must not be null or empty.</param>
    /// <param name="description">Longer description. Must not be null or empty.</param>
    /// <returns>The published <see cref="Event"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="title"/> or <paramref name="description"/> is null or empty.</exception>
    public Event Publish(NewsType newsType, string title, string description)
    {
        var @event = new Event(newsType, title, description, _dateTimeProvider.UtcNow);
        EventPublished?.Invoke(this, @event);
        return @event;
    }
}
