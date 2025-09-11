using System;

namespace Shipstone.Extensions.Notifications;

public interface INotificationConsumer
{
    String Token { get; }
    String Type { get; }
}
