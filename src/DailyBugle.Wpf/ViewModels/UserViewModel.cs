using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyBugle.Domain.Abstractions;
using DailyBugle.Domain.Entities;
using DailyBugle.Domain.Enums;
using DailyBugle.Engine;
using DailyBugle.Engine.Services;

namespace DailyBugle.Wpf.ViewModels;

/// <summary>
/// Backs the User tab: the acting user's own alert rules, "Add New Rule", and per-user notification
/// history (see ARCHITECTURE.md &#167;10, DECISION_LOG.md D-012/D-013).
/// </summary>
public sealed partial class UserViewModel : ObservableObject
{
    private readonly AlertRuleService _alertRuleService;
    private readonly IDeliveryRecordRepository _deliveryRecordRepository;
    private readonly AlertDispatcher _alertDispatcher;
    private readonly EventNewsTypeCache _eventNewsTypeCache;

    private Guid? _currentUserId;

    /// <summary>The acting user's own rules.</summary>
    public ObservableCollection<RuleRowViewModel> Rules { get; } = new();

    /// <summary>The acting user's own delivery history, most recent first.</summary>
    public ObservableCollection<DeliveryRowViewModel> History { get; } = new();

    /// <summary>All available <see cref="Enums.NewsType"/> values for the "Add New Rule" picker.</summary>
    public IReadOnlyList<NewsType> NewsTypes { get; } = Enum.GetValues<NewsType>();

    /// <summary>All available <see cref="Enums.ChannelType"/> values for the "Add New Rule" picker.</summary>
    public IReadOnlyList<ChannelType> Channels { get; } = Enum.GetValues<ChannelType>();

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private NewsType _selectedNewsType = NewsType.Science;

    [ObservableProperty]
    private ChannelType _selectedChannel = ChannelType.Email;

    [ObservableProperty]
    private string? _statusMessage;

    /// <summary>Creates a new <see cref="UserViewModel"/> and subscribes to live dispatch results.</summary>
    public UserViewModel(
        AlertRuleService alertRuleService,
        IDeliveryRecordRepository deliveryRecordRepository,
        AlertDispatcher alertDispatcher,
        EventNewsTypeCache eventNewsTypeCache)
    {
        _alertRuleService = alertRuleService ?? throw new ArgumentNullException(nameof(alertRuleService));
        _deliveryRecordRepository = deliveryRecordRepository ?? throw new ArgumentNullException(nameof(deliveryRecordRepository));
        _alertDispatcher = alertDispatcher ?? throw new ArgumentNullException(nameof(alertDispatcher));
        _eventNewsTypeCache = eventNewsTypeCache ?? throw new ArgumentNullException(nameof(eventNewsTypeCache));

        _alertDispatcher.DispatchCompleted += OnDispatchCompleted;
    }

    /// <summary>
    /// Switches the tab's data to the given identity. Called by <see cref="MainViewModel"/> whenever
    /// the global "Acting as" selector changes; a no-op display-wise when <paramref name="identity"/>
    /// is the Admin pseudo-identity (User tab is hidden in that case, see D-013).
    /// </summary>
    public void LoadForIdentity(IdentityOption identity)
    {
        _currentUserId = identity.User?.Id;
        DisplayName = identity.User?.Name ?? string.Empty;
        StatusMessage = null;
        RefreshRules();
        RefreshHistory();
    }

    private void RefreshRules()
    {
        Rules.Clear();
        if (_currentUserId is not { } userId)
        {
            return;
        }

        foreach (var rule in _alertRuleService.GetRulesForUser(userId).OrderByDescending(r => r.CreatedAt))
        {
            Rules.Add(new RuleRowViewModel(rule.Id, rule.NewsType, rule.Channel, rule.IsActive));
        }
    }

    private void RefreshHistory()
    {
        History.Clear();
        if (_currentUserId is not { } userId)
        {
            return;
        }

        foreach (var record in _deliveryRecordRepository.GetByUserId(userId))
        {
            History.Add(ToRow(record));
        }
    }

    private DeliveryRowViewModel ToRow(DeliveryRecord record) =>
        new(record.OccurredAt, _eventNewsTypeCache.TryGet(record.EventId), record.Channel, record.Success, record.ErrorMessage);

    /// <summary>
    /// Reactively prepends new history rows for the acting user as dispatch cycles complete, instead
    /// of requiring a manual tab switch/refresh to see fresh results.
    /// </summary>
    private void OnDispatchCompleted(object? sender, DispatchCompletedEventArgs args)
    {
        if (_currentUserId is not { } userId)
        {
            return;
        }

        var relevant = args.Records.Where(r => r.UserId == userId).ToList();
        if (relevant.Count == 0)
        {
            return;
        }

        // AlertDispatcher raises this from a background continuation (post-await), so marshal back
        // to the UI thread before touching the bound ObservableCollection.
        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            foreach (var record in relevant)
            {
                History.Insert(0, ToRow(record));
            }
        });
    }

    private bool CanModifyRules() => _currentUserId is not null;

    /// <summary>Adds a new active <see cref="AlertRule"/> for the acting user with the selected news type/channel.</summary>
    [RelayCommand(CanExecute = nameof(CanModifyRules))]
    private void AddRule()
    {
        if (_currentUserId is not { } userId)
        {
            return;
        }

        try
        {
            _alertRuleService.AddRule(userId, SelectedNewsType, SelectedChannel);
            RefreshRules();
            StatusMessage = $"Added rule: {SelectedNewsType} via {SelectedChannel}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to add rule: {ex.Message}";
        }
    }

    /// <summary>Removes the given rule for the acting user.</summary>
    [RelayCommand]
    private void RemoveRule(RuleRowViewModel? row)
    {
        if (row is null)
        {
            return;
        }

        try
        {
            _alertRuleService.RemoveRule(row.Id);
            RefreshRules();
            StatusMessage = $"Removed rule: {row.NewsType} via {row.Channel}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to remove rule: {ex.Message}";
        }
    }
}
