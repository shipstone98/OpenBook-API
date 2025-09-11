using System;

namespace Shipstone.OpenBook.Api.Infrastructure.Notifications;

/// <summary>
/// Represents the exception that is thrown when notifications can not be sent.
/// </summary>
public class NotificationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationException" /> class.
    /// </summary>
    public NotificationException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error, or <c>null</c>.</param>
    public NotificationException(String? message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error, or <c>null</c>.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c>.</param>
    public NotificationException(String? message, Exception? innerException)
        : base(message, innerException)
    { }
}
