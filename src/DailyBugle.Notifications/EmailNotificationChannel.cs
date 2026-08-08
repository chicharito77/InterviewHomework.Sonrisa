using System.Net;
using System.Net.Mail;
using DailyBugle.Domain.Entities;
using DailyBugle.Domain.Enums;
using DailyBugle.Notifications.Configuration;
using Microsoft.Extensions.Options;

namespace DailyBugle.Notifications;

/// <summary>
/// Strategy implementation delivering notifications via SMTP (Gmail by default, per DECISION_LOG.md
/// D-003), using the recipient <see cref="User.Email"/> as the target address.
/// </summary>
public sealed class EmailNotificationChannel : INotificationChannel
{
    private readonly EmailChannelOptions _options;

    /// <inheritdoc />
    public ChannelType Channel => ChannelType.Email;

    /// <summary>Creates a new <see cref="EmailNotificationChannel"/> from the resolved <see cref="EmailChannelOptions"/>.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public EmailNotificationChannel(IOptions<EmailChannelOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options.Value;
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="user"/> has no email address configured.</exception>
    public async Task SendAsync(User user, Event @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(@event);

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new InvalidOperationException($"User '{user.Id}' has no email address configured.");
        }

        using var client = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
        {
            EnableSsl = _options.UseSsl,
            Credentials = new NetworkCredential(_options.SenderEmail, _options.SenderPassword),
            Timeout = _options.TimeoutSeconds * 1000
        };

        using var message = new MailMessage(_options.SenderEmail, user.Email)
        {
            Subject = $"[DailyBugle] {@event.NewsType}: {@event.Title}",
            Body = @event.Description
        };

        await client.SendMailAsync(message, cancellationToken).ConfigureAwait(false);
    }
}
