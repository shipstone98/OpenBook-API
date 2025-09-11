using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.Extensions.Notifications;

public interface INotificationService
{
    Task SendAsync(
        INotification notification,
        IEnumerable<INotificationConsumer> notificationConsumers,
        CancellationToken cancellationToken
    );
}
