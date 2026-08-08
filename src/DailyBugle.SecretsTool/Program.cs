using DailyBugle.Infrastructure.Security;

// DailyBugle.SecretsTool — encrypts local plaintext credentials into secrets.local.json (DPAPI,
// CurrentUser scope; see DECISION_LOG.md D-009a). Run this locally; it never prints secret values to
// the console, and it deletes the plaintext input file after a successful encrypted write.

Console.WriteLine("DailyBugle SecretsTool");
Console.WriteLine("Encrypts local plaintext credentials into an encrypted secrets.local.json.");
Console.WriteLine();

Console.Write("Path to plaintext input file (key=value lines): ");
var inputPath = Console.ReadLine()?.Trim().Trim('"');

if (string.IsNullOrWhiteSpace(inputPath) || !File.Exists(inputPath))
{
    Console.WriteLine("Input file not found. Aborting.");
    return 1;
}

var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
foreach (var line in File.ReadLines(inputPath))
{
    if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#'))
    {
        continue;
    }

    var separatorIndex = line.IndexOf('=');
    if (separatorIndex <= 0)
    {
        continue;
    }

    var key = line[..separatorIndex].Trim();
    var value = line[(separatorIndex + 1)..].Trim();
    values[key] = value;
}

string[] requiredKeys = ["GMAIL_SENDER_EMAIL", "GMAIL_APP_PASSWORD", "SLACK_WEBHOOK_URL"];
var missing = requiredKeys.Where(k => !values.TryGetValue(k, out var v) || string.IsNullOrWhiteSpace(v)).ToList();
if (missing.Count > 0)
{
    Console.WriteLine($"Missing required key(s): {string.Join(", ", missing)}. Aborting. (No values were logged.)");
    return 1;
}

var defaultOutputPath = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "DailyBugle.Wpf", "secrets.local.json"));

Console.Write($"Output path for secrets.local.json [default: {defaultOutputPath}]: ");
var outputPathInput = Console.ReadLine()?.Trim().Trim('"');
var outputPath = string.IsNullOrWhiteSpace(outputPathInput) ? defaultOutputPath : Path.GetFullPath(outputPathInput);

var protector = new SecretProtector();
var store = new EncryptedSecretsStore(protector);

store.Save(outputPath, values["GMAIL_SENDER_EMAIL"], values["GMAIL_APP_PASSWORD"], values["SLACK_WEBHOOK_URL"]);
Console.WriteLine($"Encrypted 3 secret(s) -> {outputPath}");

try
{
    File.Delete(inputPath);
    Console.WriteLine("Plaintext input file deleted.");
}
catch (IOException ex)
{
    Console.WriteLine($"Warning: could not delete plaintext input file automatically ({ex.GetType().Name}). Please delete it manually: {inputPath}");
}

return 0;
