namespace DailyBugle.Domain.Abstractions;

/// <summary>
/// Testable abstraction over the system clock (the "DateTimeHandler" contract). Implementations
/// provide the current UTC time; tests can substitute a fixed/fake clock.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>Gets the current UTC date and time.</summary>
    DateTime UtcNow { get; }
}
