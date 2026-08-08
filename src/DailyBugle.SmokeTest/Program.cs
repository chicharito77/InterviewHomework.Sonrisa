using DailyBugle.Domain.Enums;
using DailyBugle.Engine;
using DailyBugle.Infrastructure;
using DailyBugle.Infrastructure.Repositories;
using DailyBugle.Infrastructure.Security;
using DailyBugle.Notifications;
using DailyBugle.Notifications.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

// Phase 2 manual smoke-test harness — validates real Email + Slack delivery end to end against the
// live Domain/Notifications/Engine/Infrastructure implementation, with no mocks. Kept locally as a
// reusable dev tool (not added to DailyBugle.sln, not wired into the WPF app) — see
// docs/testreports/phase2-smoke-test-attempt2.md for the reference run and results. Requires a
// valid src/DailyBugle.Wpf/secrets.local.json (see DailyBugle.SecretsTool) to run.

var secretsPath = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "DailyBugle.Wpf", "secrets.local.json"));

Console.WriteLine($"Loading secrets from: {secretsPath}");
var secrets = new EncryptedSecretsStore(new SecretProtector()).Load(secretsPath);
Console.WriteLine("Secrets loaded and decrypted in memory (values not printed).");

var dateTimeProvider = new DateTimeHandler();
var userRepository = new InMemoryUserRepository();
var alertRuleRepository = new InMemoryAlertRuleRepository();
var deliveryRecordRepository = new InMemoryDeliveryRecordRepository();

DemoDataSeeder.Seed(userRepository, alertRuleRepository, dateTimeProvider, secrets);
Console.WriteLine($"Seeded {userRepository.GetAll().Count} users.");

var emailOptions = Options.Create(new EmailChannelOptions
{
    SmtpHost = "smtp.gmail.com",
    SmtpPort = 587,
    SenderEmail = secrets.GmailSenderEmail,
    SenderPassword = secrets.GmailAppPassword,
    UseSsl = true
});

var slackOptions = Options.Create(new SlackChannelOptions());

using var httpClient = new HttpClient();

INotificationChannel[] channels =
[
    new EmailNotificationChannel(emailOptions),
    new SlackNotificationChannel(httpClient, slackOptions)
];

var resolver = new NotificationChannelResolver(channels);
var simulator = new NewsSimulator(dateTimeProvider);
var dispatcher = new AlertDispatcher(
    simulator,
    alertRuleRepository,
    userRepository,
    deliveryRecordRepository,
    resolver,
    dateTimeProvider,
    NullLogger<AlertDispatcher>.Instance);

Console.WriteLine();
Console.WriteLine("Firing a Science event (expected: both Németh [Email] and Estebán [Slack] notified)...");
var scienceEvent = simulator.Publish(
    NewsType.Science,
    "Smoke Test: Fusion Reactor Breakthrough",
    "DailyBugle smoke test — validating multi-user, multi-channel dispatch for a Science event.");

Console.WriteLine("Waiting for async dispatch (SMTP + HTTP calls) to complete...");
await Task.Delay(TimeSpan.FromSeconds(10));

Console.WriteLine();
Console.WriteLine("=== Delivery Records ===");
foreach (var user in userRepository.GetAll())
{
    var records = deliveryRecordRepository.GetByUserId(user.Id);
    if (records.Count == 0)
    {
        Console.WriteLine($"  {user.Name}: (no delivery records)");
        continue;
    }

    foreach (var record in records)
    {
        Console.WriteLine($"  {user.Name} | Channel={record.Channel} | Success={record.Success} | Error={record.ErrorMessage ?? "(none)"}");
    }
}

Console.WriteLine();
Console.WriteLine("=== Single-recipient control: firing a Sport event (expected: only Németh [Email] notified) ===");
var sportEvent = simulator.Publish(NewsType.Sport, "Smoke Test: Sport Result", "Control event — only Németh subscribes to Sport.");
await Task.Delay(TimeSpan.FromSeconds(5));

foreach (var user in userRepository.GetAll())
{
    var records = deliveryRecordRepository.GetByUserId(user.Id)
        .Where(r => r.EventId == sportEvent.Id)
        .ToList();

    Console.WriteLine(records.Count == 0
        ? $"  {user.Name}: (no delivery for Sport event, as expected if not Németh)"
        : string.Join(Environment.NewLine, records.Select(r => $"  {user.Name} | Channel={r.Channel} | Success={r.Success} | Error={r.ErrorMessage ?? "(none)"}")));
}

Console.WriteLine();
Console.WriteLine("Smoke test complete.");
