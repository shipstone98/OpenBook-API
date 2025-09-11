using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Notifications.Notifications;

namespace Shipstone.OpenBook.Api.Infrastructure.Notifications;

internal sealed class NotificationService : INotificationService
{
    private readonly Shipstone.Extensions.Notifications.INotificationService _notification;

    public NotificationService(Shipstone.Extensions.Notifications.INotificationService notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        this._notification = notification;
    }

    private async Task SendPostCreatedAsync(
        String creatorName,
        long id,
        IEnumerable<UserDeviceEntity> userDevices,
        CancellationToken cancellationToken
    )
    {
        Extensions.Notifications.INotification notification =
            new PostCreatedNotification(creatorName);

        try
        {
            await this._notification.SendAsync(
                notification,
                userDevices,
                cancellationToken
            );
        }

        catch (Extensions.Notifications.NotificationException ex)
        {
            throw new NotificationException(
                "The post created notifications could not be sent.",
                ex
            );
        }
    }

    Task INotificationService.SendPostCreatedAsync(
        String creatorName,
        long id,
        IEnumerable<UserDeviceEntity> userDevices,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(creatorName);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);
        ArgumentNullException.ThrowIfNull(userDevices);

        if (userDevices.Any(ud => ud is null))
        {
            throw new ArgumentException(
                $"{nameof (userDevices)} contains one or more elements that are null.",
                nameof (userDevices)
            );
        }

        return this.SendPostCreatedAsync(
            creatorName,
            id,
            userDevices,
            cancellationToken
        );
    }
}
