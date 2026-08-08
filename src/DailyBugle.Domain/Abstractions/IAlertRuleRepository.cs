using DailyBugle.Domain.Entities;
using DailyBugle.Domain.Enums;

namespace DailyBugle.Domain.Abstractions;

/// <summary>
/// Repository abstraction over <see cref="AlertRule"/> persistence.
/// </summary>
public interface IAlertRuleRepository
{
    /// <summary>Returns the <see cref="AlertRule"/> with the given <paramref name="id"/>, or null if not found.</summary>
    AlertRule? GetById(Guid id);

    /// <summary>Returns all rules (active and inactive) owned by the given user.</summary>
    IReadOnlyCollection<AlertRule> GetByUserId(Guid userId);

    /// <summary>Returns all currently active rules subscribed to <paramref name="newsType"/>, across all users.</summary>
    IReadOnlyCollection<AlertRule> GetActiveByNewsType(NewsType newsType);

    /// <summary>Adds a new rule to the repository.</summary>
    void Add(AlertRule rule);

    /// <summary>
    /// Removes the rule with the given <paramref name="id"/>.
    /// </summary>
    /// <exception cref="Exceptions.AlertRuleNotFoundException">Thrown when no rule with <paramref name="id"/> exists.</exception>
    void Remove(Guid id);
}
