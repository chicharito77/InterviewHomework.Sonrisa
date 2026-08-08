using DailyBugle.Domain.Abstractions;
using DailyBugle.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace DailyBugle.Engine;

/// <summary>
/// Observer that subscribes to an <see cref="IEventPublisher"/> (e.g. <see cref="NewsSimulator"/>),
/// resolves matching active <see cref="AlertRule"/>s for each published <see cref="Event"/>,
/// dispatches through the Strategy-selected notification channel, and persists exactly one
/// <see cref="DeliveryRecord"/> per dispatch attempt (success or failure) via
/// <see cref="IDeliveryRecordRepository"/> — this backs the User tab's per-user history panel
/// (see DECISION_LOG.md D-012).
/// </summary>
public sealed class AlertDispatcher
{
    private readonly IAlertRuleRepository _alertRuleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDeliveryRecordRepository _deliveryRecordRepository;
    private readonly NotificationChannelResolver _channelResolver;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<AlertDispatcher> _logger;

    /// <summary>
    /// Raised after all matching <see cref="AlertRule"/>s for one published <see cref="Event"/> have
    /// been processed (each attempt already recorded via <see cref="IDeliveryRecordRepository"/>).
    /// Purely a UI convenience — e.g. lets the Admin tab reactively show "Last Dispatch Results"
    /// without polling or an artificial delay. <see cref="AlertDispatcher"/>'s own matching/dispatch
    /// logic does not depend on this event in any way.
    /// </summary>
    public event EventHandler<DispatchCompletedEventArgs>? DispatchCompleted;

    /// <summary>
    /// Creates a new <see cref="AlertDispatcher"/> and subscribes to <paramref name="eventPublisher"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when any constructor argument is null.</exception>
    public AlertDispatcher(
        IEventPublisher eventPublisher,
        IAlertRuleRepository alertRuleRepository,
        IUserRepository userRepository,
        IDeliveryRecordRepository deliveryRecordRepository,
        NotificationChannelResolver channelResolver,
        IDateTimeProvider dateTimeProvider,
        ILogger<AlertDispatcher> logger)
    {
        ArgumentNullException.ThrowIfNull(eventPublisher);
        _alertRuleRepository = alertRuleRepository ?? throw new ArgumentNullException(nameof(alertRuleRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _deliveryRecordRepository = deliveryRecordRepository ?? throw new ArgumentNullException(nameof(deliveryRecordRepository));
        _channelResolver = channelResolver ?? throw new ArgumentNullException(nameof(channelResolver));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        eventPublisher.EventPublished += OnEventPublished;
    }

    /// <remarks>
    /// Intentional <c>async void</c> — a documented exception to the "no fire-and-forget" convention.
    /// <see cref="IEventPublisher.EventPublished"/> is a plain synchronous multicast delegate
    /// (Observer/Pub-Sub contract, see ARCHITECTURE.md &#167;4) and cannot return a <see cref="Task"/>
    /// for the publisher to await. <see cref="DispatchAsync"/> already catches and logs every
    /// per-rule delivery failure internally; the try/catch here is a final safety net so a truly
    /// unexpected failure (e.g. a repository bug) is logged rather than escaping to the event source
    /// and crashing the process.
    /// </remarks>
    private async void OnEventPublished(object? sender, Event @event)
    {
        try
        {
            await DispatchAsync(@event).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unexpected failure while dispatching Event {EventId}.", @event.Id);
        }
    }

    /// <summary>
    /// Resolves all active <see cref="AlertRule"/>s matching <paramref name="event"/>'s news type and
    /// attempts delivery for each, recording one <see cref="DeliveryRecord"/> per attempt. Individual
    /// delivery failures are caught, logged with context, and do not stop processing of remaining
    /// rules (resilient for delivery errors; missing users/channels are logged as errors and skipped).
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="event"/> is null.</exception>
    public async Task DispatchAsync(Event @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var matchingRules = _alertRuleRepository.GetActiveByNewsType(@event.NewsType);
        var records = new List<DeliveryRecord>();

        foreach (var rule in matchingRules)
        {
            var user = _userRepository.GetById(rule.UserId);
            if (user is null)
            {
                _logger.LogError(
                    "AlertRule {AlertRuleId} references missing User {UserId}; skipping dispatch for Event {EventId}.",
                    rule.Id, rule.UserId, @event.Id);
                continue;
            }

            try
            {
                var channel = _channelResolver.Resolve(rule.Channel);
                await channel.SendAsync(user, @event, cancellationToken).ConfigureAwait(false);

                var record = DeliveryRecord.Succeeded(@event.Id, user.Id, rule.Channel, _dateTimeProvider.UtcNow);
                _deliveryRecordRepository.Add(record);
                records.Add(record);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(
                    ex,
                    "Delivery failed for AlertRule {AlertRuleId}, User {UserId}, Channel {Channel}, Event {EventId}.",
                    rule.Id, user.Id, rule.Channel, @event.Id);

                var record = DeliveryRecord.Failed(@event.Id, user.Id, rule.Channel, _dateTimeProvider.UtcNow, ex.Message);
                _deliveryRecordRepository.Add(record);
                records.Add(record);
            }
        }

        DispatchCompleted?.Invoke(this, new DispatchCompletedEventArgs(@event, records));
    }
}
