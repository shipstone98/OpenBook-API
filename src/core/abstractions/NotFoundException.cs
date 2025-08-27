using System;

namespace Shipstone.OpenBook.Api.Core;

/// <summary>
/// Represents the exception that is thrown when a requested resource can not be found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException" /> class.
    /// </summary>
    public NotFoundException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error, or <c>null</c>.</param>
    public NotFoundException(String? message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error, or <c>null</c>.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c>.</param>
    public NotFoundException(String? message, Exception? innerException)
        : base(message, innerException)
    { }
}
