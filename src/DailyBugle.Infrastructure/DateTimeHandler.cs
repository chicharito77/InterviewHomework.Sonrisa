using DailyBugle.Domain.Abstractions;

namespace DailyBugle.Infrastructure;

/// <summary>
/// Production implementation of <see cref="IDateTimeProvider"/>, wrapping <see cref="DateTime.UtcNow"/>
/// so consumers (and their tests, via a fake/mock) never call the system clock directly.
/// </summary>
public sealed class DateTimeHandler : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;
}
