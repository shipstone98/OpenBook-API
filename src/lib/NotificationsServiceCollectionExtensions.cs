using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shipstone.Extensions.Notifications;

public static class NotificationsServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationsExtensions(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services.AddSingleton<INotificationService, NotificationService>();
    }
}
