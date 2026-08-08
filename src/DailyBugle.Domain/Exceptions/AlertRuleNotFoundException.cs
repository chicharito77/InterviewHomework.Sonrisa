namespace DailyBugle.Domain.Exceptions;

/// <summary>
/// Thrown when an operation targets an <see cref="Entities.AlertRule"/> identifier that does not
/// exist in the repository (e.g. removing an already-removed rule).
/// </summary>
public sealed class AlertRuleNotFoundException : Exception
{
    /// <summary>The identifier that could not be resolved to an existing rule.</summary>
    public Guid AlertRuleId { get; }

    /// <summary>Creates a new <see cref="AlertRuleNotFoundException"/> for the given <paramref name="alertRuleId"/>.</summary>
    public AlertRuleNotFoundException(Guid alertRuleId)
        : base($"AlertRule with Id '{alertRuleId}' was not found.")
    {
        AlertRuleId = alertRuleId;
    }
}
