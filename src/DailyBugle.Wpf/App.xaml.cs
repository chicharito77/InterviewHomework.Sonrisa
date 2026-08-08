using System.IO;
using System.Net.Http;
using System.Windows;
using DailyBugle.Domain.Abstractions;
using DailyBugle.Engine;
using DailyBugle.Engine.Services;
using DailyBugle.Infrastructure;
using DailyBugle.Infrastructure.Repositories;
using DailyBugle.Infrastructure.Security;
using DailyBugle.Notifications;
using DailyBugle.Notifications.Configuration;
using DailyBugle.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DailyBugle.Wpf;

/// <summary>
/// Composition root: builds the DI container, decrypts secrets, seeds demo data, and resolves
/// <see cref="MainWindow"/> (see ARCHITECTURE.md &#167;4 — Dependency Injection).
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    /// <inheritdoc />
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        DailyBugleSecrets secrets;
        try
        {
            var secretsPath = Path.Combine(AppContext.BaseDirectory, "secrets.local.json");
            secrets = new EncryptedSecretsStore(new SecretProtector()).Load(secretsPath);
        }
        catch (Exception ex)
        {
            // Fail-fast per DECISION_LOG.md D-009a: missing/undecryptable secrets must stop the app
            // with a clear message rather than silently no-op-ing at send time.
            MessageBox.Show(
                $"Failed to load encrypted secrets required to start DailyBugle:\n\n{ex.Message}\n\n" +
                "Run DailyBugle.SecretsTool to create secrets.local.json first.",
                "DailyBugle — Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(-1);
            return;
        }

        var services = new ServiceCollection();
        ConfigureServices(services, secrets);
        _serviceProvider = services.BuildServiceProvider();

        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var alertRuleRepository = _serviceProvider.GetRequiredService<IAlertRuleRepository>();
        var dateTimeProvider = _serviceProvider.GetRequiredService<IDateTimeProvider>();
        DemoDataSeeder.Seed(userRepository, alertRuleRepository, dateTimeProvider, secrets);

        // Resolving AlertDispatcher eagerly guarantees it is subscribed to NewsSimulator.EventPublished
        // before the window is shown, regardless of which ViewModel would otherwise trigger it first.
        _serviceProvider.GetRequiredService<AlertDispatcher>();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    /// <inheritdoc />
    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services, DailyBugleSecrets secrets)
    {
        services.AddLogging(builder => builder.AddDebug());

        services.AddSingleton<IDateTimeProvider, DateTimeHandler>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IAlertRuleRepository, InMemoryAlertRuleRepository>();
        services.AddSingleton<IDeliveryRecordRepository, InMemoryDeliveryRecordRepository>();

        services.AddSingleton<HttpClient>();
        services.AddSingleton<IOptions<EmailChannelOptions>>(Options.Create(new EmailChannelOptions
        {
            SmtpHost = "smtp.gmail.com",
            SmtpPort = 587,
            SenderEmail = secrets.GmailSenderEmail,
            SenderPassword = secrets.GmailAppPassword,
            UseSsl = true
        }));
        services.AddSingleton<IOptions<SlackChannelOptions>>(Options.Create(new SlackChannelOptions()));

        services.AddSingleton<INotificationChannel, EmailNotificationChannel>();
        services.AddSingleton<INotificationChannel, SlackNotificationChannel>();
        services.AddSingleton<NotificationChannelResolver>();

        services.AddSingleton<NewsSimulator>();
        services.AddSingleton<IEventPublisher>(sp => sp.GetRequiredService<NewsSimulator>());
        services.AddSingleton<AlertDispatcher>();

        services.AddSingleton<UserService>();
        services.AddSingleton<AlertRuleService>();
        services.AddSingleton<EventNewsTypeCache>();

        services.AddSingleton<AdminViewModel>();
        services.AddSingleton<UserViewModel>();
        services.AddSingleton<MainViewModel>();

        services.AddSingleton<MainWindow>();
    }
}

