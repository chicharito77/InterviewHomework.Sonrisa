using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace DailyBugle.Infrastructure.Security;

/// <summary>
/// Encrypts/decrypts secret strings at rest using Windows DPAPI (<see cref="ProtectedData"/>,
/// <see cref="DataProtectionScope.CurrentUser"/>) — see DECISION_LOG.md D-009a. Ciphertext produced
/// by <see cref="Protect"/> is safe to persist to a local, gitignored file (e.g. <c>secrets.local.json</c>);
/// plaintext only ever exists transiently in memory after <see cref="Unprotect"/> decrypts it at
/// startup. Tied to the current Windows user account — no separate key/credential management needed.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class SecretProtector
{
    // Additional entropy scopes ciphertext to this application; DPAPI already scopes to the OS user account.
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("DailyBugle.SecretProtector.v1");

    /// <summary>Encrypts <paramref name="plaintext"/>, returning a Base64-encoded ciphertext string.</summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="plaintext"/> is null or empty.</exception>
    public string Protect(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext))
        {
            throw new ArgumentException("Plaintext must not be null or empty.", nameof(plaintext));
        }

        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var cipherBytes = ProtectedData.Protect(plaintextBytes, Entropy, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(cipherBytes);
    }

    /// <summary>Decrypts a Base64-encoded ciphertext previously produced by <see cref="Protect"/>.</summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="ciphertext"/> is null, empty, or not valid Base64.</exception>
    public string Unprotect(string ciphertext)
    {
        if (string.IsNullOrEmpty(ciphertext))
        {
            throw new ArgumentException("Ciphertext must not be null or empty.", nameof(ciphertext));
        }

        byte[] cipherBytes;
        try
        {
            cipherBytes = Convert.FromBase64String(ciphertext);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Ciphertext is not valid Base64.", nameof(ciphertext), ex);
        }

        var plaintextBytes = ProtectedData.Unprotect(cipherBytes, Entropy, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(plaintextBytes);
    }
}
