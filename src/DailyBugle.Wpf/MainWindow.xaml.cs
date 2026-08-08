using System.Windows;
using DailyBugle.Wpf.ViewModels;

namespace DailyBugle.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml — pure XAML/DataContext binding, no code-behind logic (MVVM).
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>Creates a new <see cref="MainWindow"/> bound to the given <paramref name="viewModel"/>.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public MainWindow(MainViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        InitializeComponent();
        DataContext = viewModel;
    }
}