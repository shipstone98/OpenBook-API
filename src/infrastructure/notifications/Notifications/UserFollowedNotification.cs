using System;

using Shipstone.Extensions.Notifications;

namespace Shipstone.OpenBook.Api.Infrastructure.Notifications.Notifications;

internal sealed class UserFollowedNotification : INotification
{
    private readonly String _userName;

    String INotification.Body => $"{this._userName} followed you.";
    String INotification.Title => "User Followed";

    internal UserFollowedNotification(String userName) =>
        this._userName = userName;
}
