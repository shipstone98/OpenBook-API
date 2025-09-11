using System;

namespace Shipstone.Extensions.Notifications;

public class NotificationException : Exception
{
    public NotificationException() { }

    public NotificationException(String? message) : base(message) { }

    public NotificationException(String? message, Exception? innerException)
        : base(message, innerException)
    { }
}
