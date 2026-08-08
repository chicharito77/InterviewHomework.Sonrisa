using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyBugle.Domain.Enums;
using DailyBugle.Engine;
using DailyBugle.Engine.Services;

namespace DailyBugle.Wpf.ViewModels;

/// <summary>
/// Backs the Admin tab: registered users list and "Fire Event" + "Last Dispatch Results" (see
/// ARCHITECTURE.md &#167;10, DECISION_LOG.md D-008/D-013).
/// </summary>
public sealed partial class AdminViewModel : ObservableObject
{
    private readonly UserService _userService;
    private readonly AlertRuleService _alertRuleService;
    private readonly NewsSimulator _newsSimulator;
    private readonly AlertDispatcher _alertDispatcher;

    /// <summary>All registered users, refreshed on load.</summary>
    public ObservableCollection<UserRowViewModel> Users { get; } = new();

    /// <summary>Per-recipient outcomes of the most recently fired event.</summary>
    public ObservableCollection<DeliveryRowViewModel> LastDispatchResults { get; } = new();

    /// <summary>All available <see cref="Enums.NewsType"/> values for the "Fire Event" picker.</summary>
    public IReadOnlyList<NewsType> NewsTypes { get; } = Enum.GetValues<NewsType>();

    [ObservableProperty]
    private NewsType _selectedNewsType = NewsType.Science;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _statusMessage;

    /// <summary>Creates a new <see cref="AdminViewModel"/> and loads the current user list.</summary>
    public AdminViewModel(
        UserService userService,
        AlertRuleService alertRuleService,
        NewsSimulator newsSimulator,
        AlertDispatcher alertDispatcher)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _alertRuleService = alertRuleService ?? throw new ArgumentNullException(nameof(alertRuleService));
        _newsSimulator = newsSimulator ?? throw new ArgumentNullException(nameof(newsSimulator));
        _alertDispatcher = alertDispatcher ?? throw new ArgumentNullException(nameof(alertDispatcher));

        RefreshUsers();
    }

    /// <summary>Reloads <see cref="Users"/> from the repository (also called after firing an event, in case rules changed).</summary>
    public void RefreshUsers()
    {
        Users.Clear();
        foreach (var user in _userService.GetAllUsers())
        {
            var activeRules = _alertRuleService.GetRulesForUser(user.Id).Where(r => r.IsActive).ToList();
            var channels = string.Join(", ", activeRules.Select(r => r.Channel).Distinct());
            var topics = string.Join(", ", activeRules.Select(r => r.NewsType).Distinct());
            Users.Add(new UserRowViewModel(user.Name, channels, topics));
        }
    }

    private bool CanFireEvent() => !IsBusy;

    /// <summary>
    /// Publishes a new <see cref="Domain.Entities.Event"/> via <see cref="NewsSimulator"/> and awaits
    /// <see cref="AlertDispatcher.DispatchCompleted"/> to populate <see cref="LastDispatchResults"/>
    /// with the real outcome of every matching rule (no artificial delay/poll — see
    /// docs/prompts session notes for this design decision).
    /// </summary>
    /// <remarks>
    /// The completion handler does not filter by event Id: the "Fire Event" button is disabled
    /// (<see cref="CanFireEvent"/>) while a dispatch is in flight, so at most one dispatch cycle can
    /// be outstanding at a time and any <see cref="AlertDispatcher.DispatchCompleted"/> received while
    /// awaiting is necessarily the one just fired.
    /// </remarks>
    [RelayCommand(CanExecute = nameof(CanFireEvent))]
    private async Task FireEventAsync()
    {
        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Description))
        {
            StatusMessage = "Title and description are required.";
            return;
        }

        IsBusy = true;
        StatusMessage = null;

        var completionSource = new TaskCompletionSource<DispatchCompletedEventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);
        void OnDispatchCompleted(object? sender, DispatchCompletedEventArgs args) => completionSource.TrySetResult(args);

        _alertDispatcher.DispatchCompleted += OnDispatchCompleted;
        try
        {
            _newsSimulator.Publish(SelectedNewsType, Title, Description);
            var completed = await completionSource.Task.WaitAsync(TimeSpan.FromSeconds(20)).ConfigureAwait(true);

            LastDispatchResults.Clear();
            foreach (var record in completed.Records)
            {
                var userName = _userService.GetAllUsers().FirstOrDefault(u => u.Id == record.UserId)?.Name ?? "(unknown user)";
                LastDispatchResults.Add(new DeliveryRowViewModel(
                    record.OccurredAt, completed.Event.NewsType, record.Channel, record.Success, record.ErrorMessage, userName));
            }

            StatusMessage = completed.Records.Count == 0
                ? "Event published — no subscribers matched this news type."
                : $"Dispatched to {completed.Records.Count} recipient(s).";
        }
        catch (TimeoutException)
        {
            StatusMessage = "Timed out waiting for dispatch delivery to complete.";
        }
        finally
        {
            _alertDispatcher.DispatchCompleted -= OnDispatchCompleted;
            IsBusy = false;
        }
    }

    partial void OnIsBusyChanged(bool value) => FireEventCommand.NotifyCanExecuteChanged();
}
