using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DailyBugle.Wpf.Converters;

/// <summary>
/// Inverse of the built-in <see cref="System.Windows.Controls.BooleanToVisibilityConverter"/> — used
/// to hide the User tab while <c>IsAdminModeActive</c> is true (see D-013, exclusive tab visibility).
/// </summary>
public sealed class InverseBooleanToVisibilityConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? Visibility.Collapsed : Visibility.Visible;

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">Always thrown; one-way converter only.</exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException($"{nameof(InverseBooleanToVisibilityConverter)} only supports one-way binding.");
}
