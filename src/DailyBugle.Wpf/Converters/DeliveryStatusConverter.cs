using System.Globalization;
using System.Windows.Data;

namespace DailyBugle.Wpf.Converters;

/// <summary>Renders a delivery outcome as "✔ Delivered" or "✘ Failed: &lt;reason&gt;" (see ARCHITECTURE.md &#167;10 wireframes).</summary>
public sealed class DeliveryStatusConverter : IMultiValueConverter
{
    /// <summary>Combines a <c>bool Success</c> and <c>string? ErrorMessage</c> pair into a display string.</summary>
    public object Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is not [bool success, ..])
        {
            return string.Empty;
        }

        if (success)
        {
            return "✔ Delivered";
        }

        var errorMessage = values.Length > 1 ? values[1] as string : null;
        return string.IsNullOrWhiteSpace(errorMessage) ? "✘ Failed" : $"✘ Failed: {errorMessage}";
    }

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">Always thrown; one-way converter only.</exception>
    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException($"{nameof(DeliveryStatusConverter)} only supports one-way binding.");
}
