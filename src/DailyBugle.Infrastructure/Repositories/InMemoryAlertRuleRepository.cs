using System.Collections.Concurrent;
using DailyBugle.Domain.Abstractions;
using DailyBugle.Domain.Entities;
using DailyBugle.Domain.Enums;
using DailyBugle.Domain.Exceptions;

namespace DailyBugle.Infrastructure.Repositories;

/// <summary>
/// Thread-safe in-memory implementation of <see cref="IAlertRuleRepository"/>, backed by a
/// <see cref="ConcurrentDictionary{TKey,TValue}"/> (see DECISION_LOG.md D-002 — no external database).
/// </summary>
public sealed class InMemoryAlertRuleRepository : IAlertRuleRepository
{
    private readonly ConcurrentDictionary<Guid, AlertRule> _rules = new();

    /// <inheritdoc />
    public AlertRule? GetById(Guid id) => _rules.TryGetValue(id, out var rule) ? rule : null;

    /// <inheritdoc />
    public IReadOnlyCollection<AlertRule> GetByUserId(Guid userId) =>
        _rules.Values.Where(r => r.UserId == userId).ToList();

    /// <inheritdoc />
    public IReadOnlyCollection<AlertRule> GetActiveByNewsType(NewsType newsType) =>
        _rules.Values.Where(r => r.Matches(newsType)).ToList();

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rule"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a rule with the same Id is already registered.</exception>
    public void Add(AlertRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        if (!_rules.TryAdd(rule.Id, rule))
        {
            throw new ArgumentException($"An AlertRule with Id '{rule.Id}' is already registered.", nameof(rule));
        }
    }

    /// <inheritdoc />
    /// <exception cref="AlertRuleNotFoundException">Thrown when no rule with <paramref name="id"/> exists.</exception>
    public void Remove(Guid id)
    {
        if (!_rules.TryRemove(id, out _))
        {
            throw new AlertRuleNotFoundException(id);
        }
    }
}
