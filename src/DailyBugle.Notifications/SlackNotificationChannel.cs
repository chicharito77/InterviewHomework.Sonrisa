using System.Text;
using System.Text.Json;
using DailyBugle.Domain.Entities;
using DailyBugle.Domain.Enums;
using DailyBugle.Notifications.Configuration;
using Microsoft.Extensions.Options;

namespace DailyBugle.Notifications;

/// <summary>
/// Strategy implementation delivering notifications via a Slack Incoming Webhook, using the
/// recipient's <see cref="User.SlackWebhookUrl"/> as the POST target (per DECISION_LOG.md D-003).
/// </summary>
public sealed class SlackNotificationChannel : INotificationChannel
{
    private readonly HttpClient _httpClient;
    private readonly SlackChannelOptions _options;

    /// <inheritdoc />
    public ChannelType Channel => ChannelType.Slack;

    /// <summary>Creates a new <see cref="SlackNotificationChannel"/>.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> or <paramref name="options"/> is null.</exception>
    public SlackNotificationChannel(HttpClient httpClient, IOptions<SlackChannelOptions> options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = httpClient;
        _options = options.Value;
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="user"/> has no Slack webhook configured.</exception>
    public async Task SendAsync(User user, Event @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(@event);

        if (string.IsNullOrWhiteSpace(user.SlackWebhookUrl))
        {
            throw new InvalidOperationException($"User '{user.Id}' has no Slack webhook configured.");
        }

        var payload = new
        {
            username = _options.BotUsername,
            text = $"*[{@event.NewsType}] {@event.Title}*\n{@event.Description}"
        };

        var json = JsonSerializer.Serialize(payload);

        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(_options.TimeoutSeconds));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient
            .PostAsync(user.SlackWebhookUrl, content, linkedCts.Token)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
    }
}
