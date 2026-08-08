using DailyBugle.Domain.Abstractions;
using DailyBugle.Domain.Entities;
using DailyBugle.Domain.Enums;
using DailyBugle.Infrastructure.Security;

namespace DailyBugle.Infrastructure;

/// <summary>
/// Seeds the two fixed demo users and their four <see cref="AlertRule"/>s at application startup
/// (see DECISION_LOG.md D-011/D-014) — there is no registration UI in this POC. Contact targets
/// (email, Slack webhook) are supplied via <see cref="DailyBugleSecrets"/>, resolved from the
/// encrypted local secret store; only structural seed metadata (name, news type, channel) lives in
/// source.
/// </summary>
public static class DemoDataSeeder
{
    /// <summary>Németh István's fixed seed identifier (Email; Sport + Science).</summary>
    public static readonly Guid NemethIstvanId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    /// <summary>Estebán Alemán's fixed seed identifier (Slack; Science + Economy).</summary>
    public static readonly Guid EstebanAlemanId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    /// <summary>
    /// Populates <paramref name="userRepository"/> and <paramref name="alertRuleRepository"/> with the
    /// two demo users and their four rules. Idempotent guard: does nothing if Németh István already
    /// exists (e.g. if called twice against the same repository instances).
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static void Seed(
        IUserRepository userRepository,
        IAlertRuleRepository alertRuleRepository,
        IDateTimeProvider dateTimeProvider,
        DailyBugleSecrets secrets)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        ArgumentNullException.ThrowIfNull(alertRuleRepository);
        ArgumentNullException.ThrowIfNull(dateTimeProvider);
        ArgumentNullException.ThrowIfNull(secrets);

        if (userRepository.GetById(NemethIstvanId) is not null)
        {
            return;
        }

        var nemeth = new User(NemethIstvanId, "Németh István", secrets.GmailSenderEmail);
        var esteban = new User(EstebanAlemanId, "Estebán Alemán", email: secrets.GmailSenderEmail, slackWebhookUrl: secrets.SlackWebhookUrl);

        userRepository.Add(nemeth);
        userRepository.Add(esteban);

        var now = dateTimeProvider.UtcNow;

        alertRuleRepository.Add(new AlertRule(nemeth.Id, NewsType.Sport, ChannelType.Email, now));
        alertRuleRepository.Add(new AlertRule(nemeth.Id, NewsType.Science, ChannelType.Email, now));
        alertRuleRepository.Add(new AlertRule(esteban.Id, NewsType.Science, ChannelType.Slack, now));
        alertRuleRepository.Add(new AlertRule(esteban.Id, NewsType.Economy, ChannelType.Slack, now));
    }
}
