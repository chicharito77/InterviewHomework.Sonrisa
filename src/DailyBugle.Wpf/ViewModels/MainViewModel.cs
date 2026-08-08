using CommunityToolkit.Mvvm.ComponentModel;
using DailyBugle.Engine.Services;

namespace DailyBugle.Wpf.ViewModels;

/// <summary>
/// Root view model owning the global "Acting as" identity switcher (see ARCHITECTURE.md &#167;10,
/// DECISION_LOG.md D-013). Switching identity toggles which tab (<see cref="AdminViewModel"/> vs
/// <see cref="UserViewModel"/>) is visible and reloads the User tab's data for the newly selected user.
/// </summary>
public sealed partial class MainViewModel : ObservableObject
{
    /// <summary>All selectable identities: Admin followed by every registered user.</summary>
    public IReadOnlyList<IdentityOption> Identities { get; }

    /// <summary>View model backing the Admin tab.</summary>
    public AdminViewModel AdminVm { get; }

    /// <summary>View model backing the User tab.</summary>
    public UserViewModel UserVm { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAdminModeActive))]
    private IdentityOption _selectedIdentity;

    /// <summary>True when the Admin pseudo-identity is selected (Admin tab visible, User tab hidden).</summary>
    public bool IsAdminModeActive => SelectedIdentity.User is null;

    /// <summary>Creates a new <see cref="MainViewModel"/>, seeding the identity list from registered users.</summary>
    public MainViewModel(UserService userService, AdminViewModel adminVm, UserViewModel userVm)
    {
        ArgumentNullException.ThrowIfNull(userService);
        AdminVm = adminVm ?? throw new ArgumentNullException(nameof(adminVm));
        UserVm = userVm ?? throw new ArgumentNullException(nameof(userVm));

        var identities = new List<IdentityOption> { IdentityOption.Admin };
        identities.AddRange(userService.GetAllUsers().Select(u => new IdentityOption(u.Name, u)));
        Identities = identities;

        _selectedIdentity = identities[0];
        UserVm.LoadForIdentity(_selectedIdentity);
    }

    partial void OnSelectedIdentityChanged(IdentityOption value) => UserVm.LoadForIdentity(value);
}
