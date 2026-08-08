using DailyBugle.Domain.Abstractions;
using DailyBugle.Domain.Entities;
using DailyBugle.Domain.Exceptions;

namespace DailyBugle.Engine.Services;

/// <summary>
/// Application-layer facade over <see cref="IUserRepository"/> for the presentation layer (Admin tab
/// user listing, "Acting as" identity resolution).
/// </summary>
public sealed class UserService
{
    private readonly IUserRepository _userRepository;

    /// <summary>Creates a new <see cref="UserService"/>.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="userRepository"/> is null.</exception>
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>Returns all registered users.</summary>
    public IReadOnlyCollection<User> GetAllUsers() => _userRepository.GetAll();

    /// <summary>
    /// Returns the user with the given <paramref name="userId"/>.
    /// </summary>
    /// <exception cref="UserNotFoundException">Thrown when no user with <paramref name="userId"/> exists.</exception>
    public User GetUserById(Guid userId) =>
        _userRepository.GetById(userId) ?? throw new UserNotFoundException(userId);
}
