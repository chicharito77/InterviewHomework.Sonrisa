using DailyBugle.Domain.Entities;

namespace DailyBugle.Domain.Abstractions;

/// <summary>
/// Observer/Pub-Sub contract for the news feed. Publishers (e.g. <c>NewsSimulator</c>) raise
/// <see cref="EventPublished"/> without any knowledge of subscribers (e.g. <c>AlertDispatcher</c>),
/// decoupling event production from consumption.
/// </summary>
public interface IEventPublisher
{
    /// <summary>Raised whenever a new <see cref="Event"/> is published. Null until the first subscriber attaches.</summary>
    event EventHandler<Event>? EventPublished;
}
