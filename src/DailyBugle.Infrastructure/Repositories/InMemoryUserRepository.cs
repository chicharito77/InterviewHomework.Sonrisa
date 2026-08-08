using System.Collections.Concurrent;
using DailyBugle.Domain.Abstractions;
using DailyBugle.Domain.Entities;

namespace DailyBugle.Infrastructure.Repositories;

/// <summary>
/// Thread-safe in-memory implementation of <see cref="IUserRepository"/>, backed by a
/// <see cref="ConcurrentDictionary{TKey,TValue}"/> (see DECISION_LOG.md D-002 — no external database).
/// </summary>
public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();

    /// <inheritdoc />
    public User? GetById(Guid id) => _users.TryGetValue(id, out var user) ? user : null;

    /// <inheritdoc />
    public IReadOnlyCollection<User> GetAll() => _users.Values.ToList();

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="user"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a user with the same Id is already registered.</exception>
    public void Add(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (!_users.TryAdd(user.Id, user))
        {
            throw new ArgumentException($"A user with Id '{user.Id}' is already registered.", nameof(user));
        }
    }
}
