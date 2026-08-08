namespace DailyBugle.Wpf.ViewModels;

/// <summary>Read-only row shown in the Admin tab's "Registered Users" list.</summary>
public sealed record UserRowViewModel(string Name, string Channels, string SubscribedNewsTypes);
