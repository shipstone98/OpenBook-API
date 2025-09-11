using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Notifications;

namespace Shipstone.OpenBook.Api.Infrastructure.NotificationsTest.Mocks;

internal sealed class MockNotificationService : INotificationService
{
    internal Action<INotification, IEnumerable<INotificationConsumer>> _sendAction;

    public MockNotificationService() =>
        this._sendAction = (_, _) => throw new NotImplementedException();

    Task INotificationService.SendAsync(
        INotification notification,
        IEnumerable<INotificationConsumer> notificationConsumers,
        CancellationToken cancellationToken
    )
    {
        this._sendAction(notification, notificationConsumers);
        return Task.CompletedTask;
    }
}
