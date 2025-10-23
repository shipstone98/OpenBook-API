using System;

using Shipstone.Extensions.Notifications;

namespace Shipstone.OpenBook.Api.Infrastructure.Notifications.Notifications;

internal sealed class UserUnfollowedNotification : INotification
{
    private readonly String _userName;

    String INotification.Body => $"{this._userName} unfollowed you.";
    String INotification.Title => "User Unfollowed";

    internal UserUnfollowedNotification(String userName) =>
        this._userName = userName;
}
