using DailyBugle.Domain.Entities;

namespace DailyBugle.Domain.Abstractions;

/// <summary>
/// Repository abstraction over <see cref="User"/> persistence. Swappable implementation
/// (in-memory today, potentially EF Core later) without touching Domain/Engine/Wpf.
/// </summary>
public interface IUserRepository
{
    /// <summary>Returns the <see cref="User"/> with the given <paramref name="id"/>, or null if not found.</summary>
    User? GetById(Guid id);

    /// <summary>Returns all registered users.</summary>
    IReadOnlyCollection<User> GetAll();

    /// <summary>Adds a new user to the repository.</summary>
    void Add(User user);
}
