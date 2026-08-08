namespace DailyBugle.Domain.Enums;

/// <summary>
/// Categorizes a world event so it can be matched against a user's <see cref="Entities.AlertRule"/>s.
/// </summary>
public enum NewsType
{
    /// <summary>Sports-related news (e.g. match results, championships).</summary>
    Sport,

    /// <summary>Economic/market-related news (e.g. stock movements, rate changes).</summary>
    Economy,

    /// <summary>Scientific breakthroughs and discoveries.</summary>
    Science
}
