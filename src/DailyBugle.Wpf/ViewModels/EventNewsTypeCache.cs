using System.Collections.Concurrent;
using DailyBugle.Domain.Enums;
using DailyBugle.Engine;

namespace DailyBugle.Wpf.ViewModels;

/// <summary>
/// Presentation-layer-only lookup from a published <c>Event</c>'s Id to its <see cref="NewsType"/>,
/// populated reactively from <see cref="AlertDispatcher.DispatchCompleted"/>. Needed because
/// <c>DeliveryRecord</c> (Domain) intentionally only stores the triggering <c>EventId</c>, not a
/// full snapshot of the event — this cache lets the WPF history views show a NewsType column
/// without touching the Domain entity. Complete for the lifetime of the process since the
/// in-memory repositories (and therefore every <c>DeliveryRecord</c> in existence) are also
/// recreated fresh at each app startup.
/// </summary>
public sealed class EventNewsTypeCache
{
    private readonly ConcurrentDictionary<Guid, NewsType> _newsTypesByEventId = new();

    /// <summary>Creates a new <see cref="EventNewsTypeCache"/> and subscribes to <paramref name="alertDispatcher"/>.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="alertDispatcher"/> is null.</exception>
    public EventNewsTypeCache(AlertDispatcher alertDispatcher)
    {
        ArgumentNullException.ThrowIfNull(alertDispatcher);
        alertDispatcher.DispatchCompleted += (_, args) => _newsTypesByEventId[args.Event.Id] = args.Event.NewsType;
    }

    /// <summary>Returns the cached <see cref="NewsType"/> for <paramref name="eventId"/>, or null if unknown.</summary>
    public NewsType? TryGet(Guid eventId) =>
        _newsTypesByEventId.TryGetValue(eventId, out var newsType) ? newsType : null;
}
