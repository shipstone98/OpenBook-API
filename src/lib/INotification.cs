using System;

namespace Shipstone.Extensions.Notifications;

public interface INotification
{
    String Body { get; }
    String Title { get; }
}
