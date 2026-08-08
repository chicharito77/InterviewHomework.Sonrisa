using DailyBugle.Domain.Entities;

namespace DailyBugle.Engine;

/// <summary>
/// Event data raised via <see cref="AlertDispatcher.DispatchCompleted"/> once all matching
/// <see cref="AlertRule"/>s for <see cref="Event"/> have been processed.
/// </summary>
/// <param name="Event">The published event that triggered this dispatch cycle.</param>
/// <param name="Records">One <see cref="DeliveryRecord"/> per matching rule, in processing order.</param>
public sealed record DispatchCompletedEventArgs(Event Event, IReadOnlyList<DeliveryRecord> Records);
