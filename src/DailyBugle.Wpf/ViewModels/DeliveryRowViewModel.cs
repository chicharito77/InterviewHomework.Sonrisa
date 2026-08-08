using DailyBugle.Domain.Enums;

namespace DailyBugle.Wpf.ViewModels;

/// <summary>
/// Read-only row shown in the Admin tab's "Last Dispatch Results" and the User tab's "My
/// Notification History" lists. <see cref="UserName"/> is populated only for the Admin view;
/// <see cref="NewsType"/> is resolved from <see cref="EventNewsTypeCache"/> for historical
/// records (the persisted <c>DeliveryRecord</c> itself only stores the triggering event's Id).
/// </summary>
public sealed record DeliveryRowViewModel(
    DateTime When,
    NewsType? NewsType,
    ChannelType Channel,
    bool Success,
    string? ErrorMessage,
    string? UserName = null);
