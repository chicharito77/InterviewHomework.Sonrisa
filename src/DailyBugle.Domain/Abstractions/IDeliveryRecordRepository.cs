using DailyBugle.Domain.Entities;

namespace DailyBugle.Domain.Abstractions;

/// <summary>
/// Repository abstraction over <see cref="DeliveryRecord"/> persistence. Backs the User tab's
/// per-user notification history panel (see DECISION_LOG.md D-012) — not a general admin-wide log.
/// </summary>
public interface IDeliveryRecordRepository
{
    /// <summary>Returns all delivery records addressed to the given user, most recent first.</summary>
    IReadOnlyCollection<DeliveryRecord> GetByUserId(Guid userId);

    /// <summary>Adds a new delivery record (one per dispatch attempt, success or failure).</summary>
    void Add(DeliveryRecord record);
}
