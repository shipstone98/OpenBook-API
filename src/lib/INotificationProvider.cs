using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.Extensions.Notifications;

public interface INotificationProvider
{
    String Type { get; }

    Task SendAsync(
        INotification notification,
        INotificationConsumer notificationConsumer,
        CancellationToken cancellationToken
    );
}
