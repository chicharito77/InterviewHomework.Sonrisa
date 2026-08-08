using DailyBugle.Domain.Enums;

namespace DailyBugle.Wpf.ViewModels;

/// <summary>Read-only row shown in the User tab's "My Alert Rules" list.</summary>
public sealed record RuleRowViewModel(Guid Id, NewsType NewsType, ChannelType Channel, bool IsActive);
