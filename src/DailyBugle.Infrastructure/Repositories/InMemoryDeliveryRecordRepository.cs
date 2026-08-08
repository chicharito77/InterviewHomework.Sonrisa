using System.Collections.Concurrent;
using DailyBugle.Domain.Abstractions;
using DailyBugle.Domain.Entities;

namespace DailyBugle.Infrastructure.Repositories;

/// <summary>
/// Thread-safe in-memory implementation of <see cref="IDeliveryRecordRepository"/>, backed by a
/// <see cref="ConcurrentBag{T}"/> (see DECISION_LOG.md D-002 — no external database). Backs the User
/// tab's per-user notification history panel only (see DECISION_LOG.md D-012).
/// </summary>
public sealed class InMemoryDeliveryRecordRepository : IDeliveryRecordRepository
{
    private readonly ConcurrentBag<DeliveryRecord> _records = new();

    /// <inheritdoc />
    /// <remarks>Results are ordered most-recent-first for direct binding to the User tab's history list.</remarks>
    public IReadOnlyCollection<DeliveryRecord> GetByUserId(Guid userId) =>
        _records
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.OccurredAt)
            .ToList();

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> is null.</exception>
    public void Add(DeliveryRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        _records.Add(record);
    }
}
