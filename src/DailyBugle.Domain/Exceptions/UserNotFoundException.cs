namespace DailyBugle.Domain.Exceptions;

/// <summary>
/// Thrown when a lookup by user identifier fails to resolve to an existing <see cref="Entities.User"/>.
/// </summary>
public sealed class UserNotFoundException : Exception
{
    /// <summary>The identifier that could not be resolved to a user.</summary>
    public Guid UserId { get; }

    /// <summary>Creates a new <see cref="UserNotFoundException"/> for the given <paramref name="userId"/>.</summary>
    public UserNotFoundException(Guid userId)
        : base($"User with Id '{userId}' was not found.")
    {
        UserId = userId;
    }
}
