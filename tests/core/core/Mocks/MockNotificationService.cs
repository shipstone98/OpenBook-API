using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Notifications;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockNotificationService : INotificationService
{
    internal Action<String, long, IEnumerable<UserDeviceEntity>> _sendPostCreatedAction;
    internal Action<String, IEnumerable<UserDeviceEntity>> _sendUserFollowedAction;
    internal Action<String, IEnumerable<UserDeviceEntity>> _sendUserUnfollowedAction;

    public MockNotificationService()
    {
        this._sendPostCreatedAction = (_, _, _) =>
            throw new NotImplementedException();

        this._sendUserFollowedAction = (_, _) =>
            throw new NotImplementedException();

        this._sendUserUnfollowedAction = (_, _) =>
            throw new NotImplementedException();
    }

    Task INotificationService.SendPostCreatedAsync(
        String creatorName,
        long id,
        IEnumerable<UserDeviceEntity> userDevices,
        CancellationToken cancellationToken
    )
    {
        this._sendPostCreatedAction(creatorName, id, userDevices);
        return Task.CompletedTask;
    }

    Task INotificationService.SendUserFollowedAsync(
        String userName,
        IEnumerable<UserDeviceEntity> userDevices,
        CancellationToken cancellationToken
    )
    {
        this._sendUserFollowedAction(userName, userDevices);
        return Task.CompletedTask;
    }

    Task INotificationService.SendUserUnfollowedAsync(
        String userName,
        IEnumerable<UserDeviceEntity> userDevices,
        CancellationToken cancellationToken
    )
    {
        this._sendUserUnfollowedAction(userName, userDevices);
        return Task.CompletedTask;
    }
}
