using DailyBugle.Domain.Entities;

namespace DailyBugle.Wpf.ViewModels;

/// <summary>
/// One entry in the global "Acting as" identity switcher (see ARCHITECTURE.md &#167;10, D-013).
/// <see cref="User"/> is null for the UI-only Admin pseudo-identity.
/// </summary>
public sealed record IdentityOption(string DisplayName, User? User)
{
    /// <summary>The Admin pseudo-identity — not a domain <see cref="Entities.User"/>.</summary>
    public static readonly IdentityOption Admin = new("Admin", null);
}
