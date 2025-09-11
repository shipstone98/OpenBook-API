using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.Extensions.Notifications;

internal sealed class NotificationService : INotificationService
{
    private readonly IReadOnlyDictionary<String, INotificationProvider> _providers;

    public NotificationService(IEnumerable<INotificationProvider> providers)
    {
        ArgumentNullException.ThrowIfNull(providers);

        if (providers.Any(p => p is null))
        {
            throw new ArgumentException(
                $"{nameof (providers)} contains one or more elements that are null.",
                nameof (providers)
            );
        }

        this._providers = providers.ToDictionary(p => p.Type, p => p);
    }

    private async Task SendAsync(
        INotification notification,
        IEnumerable<INotificationConsumer> notificationConsumers,
        CancellationToken cancellationToken
    )
    {
        ICollection<Task> tasks = new List<Task>();

        foreach (INotificationConsumer notificationConsumer in notificationConsumers)
        {
            if (this._providers.TryGetValue(
                notificationConsumer.Type,
                out INotificationProvider? provider
            ))
            {
                Task task =
                    provider.SendAsync(
                        notification,
                        notificationConsumer,
                        cancellationToken
                    );

                tasks.Add(task);
            }
        }

        try
        {
            await Task.WhenAll(tasks);
        }

        catch (Exception ex)
        {
            throw new NotificationException(
                "The notifications could not be sent.",
                ex
            );
        }
    }

    Task INotificationService.SendAsync(
        INotification notification,
        IEnumerable<INotificationConsumer> notificationConsumers,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(notification);
        ArgumentNullException.ThrowIfNull(notificationConsumers);

        if (notificationConsumers.Any(nc => nc is null))
        {
            throw new ArgumentException(
                $"{nameof (notificationConsumers)} contains one or more elements that are null.",
                nameof (notificationConsumers)
            );
        }

        return this.SendAsync(
            notification,
            notificationConsumers,
            cancellationToken
        );
    }
}
