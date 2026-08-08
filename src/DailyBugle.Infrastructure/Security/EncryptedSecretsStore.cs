using System.Runtime.Versioning;
using System.Text.Json;

namespace DailyBugle.Infrastructure.Security;

/// <summary>
/// Loads/saves <c>secrets.local.json</c>, mediating encryption on write and decryption on read via
/// <see cref="SecretProtector"/>. Plaintext values are never written to disk.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class EncryptedSecretsStore
{
    private readonly SecretProtector _protector;

    /// <summary>Creates a new <see cref="EncryptedSecretsStore"/>.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="protector"/> is null.</exception>
    public EncryptedSecretsStore(SecretProtector protector)
    {
        _protector = protector ?? throw new ArgumentNullException(nameof(protector));
    }

    /// <summary>
    /// Encrypts the given plaintext values and writes them to <paramref name="filePath"/> as a
    /// <see cref="SecretsFile"/> JSON document, overwriting any existing file.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is null or empty.</exception>
    public void Save(string filePath, string gmailSenderEmail, string gmailAppPassword, string slackWebhookUrl)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
        }

        var file = new SecretsFile
        {
            GmailSenderEmailCipher = _protector.Protect(gmailSenderEmail),
            GmailAppPasswordCipher = _protector.Protect(gmailAppPassword),
            SlackWebhookUrlCipher = _protector.Protect(slackWebhookUrl)
        };

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(file, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Loads and decrypts <paramref name="filePath"/> into a <see cref="DailyBugleSecrets"/> instance.
    /// </summary>
    /// <exception cref="FileNotFoundException">Thrown when <paramref name="filePath"/> does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file's content cannot be parsed.</exception>
    public DailyBugleSecrets Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Secrets file not found at '{filePath}'. Run DailyBugle.SecretsTool first.", filePath);
        }

        var json = File.ReadAllText(filePath);
        var file = JsonSerializer.Deserialize<SecretsFile>(json)
            ?? throw new InvalidOperationException($"Secrets file at '{filePath}' could not be parsed.");

        return new DailyBugleSecrets(
            _protector.Unprotect(file.GmailSenderEmailCipher),
            _protector.Unprotect(file.GmailAppPasswordCipher),
            _protector.Unprotect(file.SlackWebhookUrlCipher));
    }
}
