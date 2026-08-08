using DailyBugle.Domain.Abstractions;
using DailyBugle.Domain.Entities;
using DailyBugle.Domain.Enums;
using DailyBugle.Domain.Exceptions;

namespace DailyBugle.Engine.Services;

/// <summary>
/// Application-layer facade over <see cref="IAlertRuleRepository"/> for the presentation layer (User
/// tab: list rules, add rule, remove rule).
/// </summary>
public sealed class AlertRuleService
{
    private readonly IAlertRuleRepository _alertRuleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>Creates a new <see cref="AlertRuleService"/>.</summary>
    /// <exception cref="ArgumentNullException">Thrown when any constructor argument is null.</exception>
    public AlertRuleService(
        IAlertRuleRepository alertRuleRepository,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _alertRuleRepository = alertRuleRepository ?? throw new ArgumentNullException(nameof(alertRuleRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    /// <summary>Returns all rules (active and inactive) owned by <paramref name="userId"/>.</summary>
    public IReadOnlyCollection<AlertRule> GetRulesForUser(Guid userId) =>
        _alertRuleRepository.GetByUserId(userId);

    /// <summary>
    /// Creates and persists a new active <see cref="AlertRule"/> for <paramref name="userId"/>.
    /// </summary>
    /// <exception cref="UserNotFoundException">Thrown when <paramref name="userId"/> does not resolve to an existing user.</exception>
    public AlertRule AddRule(Guid userId, NewsType newsType, ChannelType channel)
    {
        _ = _userRepository.GetById(userId) ?? throw new UserNotFoundException(userId);

        var rule = new AlertRule(userId, newsType, channel, _dateTimeProvider.UtcNow);
        _alertRuleRepository.Add(rule);
        return rule;
    }

    /// <summary>
    /// Removes the rule with the given <paramref name="ruleId"/>.
    /// </summary>
    /// <exception cref="AlertRuleNotFoundException">Thrown when no rule with <paramref name="ruleId"/> exists.</exception>
    public void RemoveRule(Guid ruleId) => _alertRuleRepository.Remove(ruleId);
}
