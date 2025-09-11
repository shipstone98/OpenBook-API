using System;

using Shipstone.Extensions.Notifications;

namespace Shipstone.OpenBook.Api.Infrastructure.Notifications.Notifications;

internal sealed class PostCreatedNotification : INotification
{
    private readonly String _creatorName;

    String INotification.Body => $"{this._creatorName} created a new post.";
    String INotification.Title => "Post created";

    internal PostCreatedNotification(String creatorName) =>
        this._creatorName = creatorName;
}
