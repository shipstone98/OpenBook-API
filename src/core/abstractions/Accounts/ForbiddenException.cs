using System;

namespace Shipstone.OpenBook.Api.Core.Accounts;

/// <summary>
/// Represents the exception that is thrown when a requested operation can not be performed.
/// </summary>
public class ForbiddenException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException" /> class.
    /// </summary>
    public ForbiddenException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error, or <c>null</c>.</param>
    public ForbiddenException(String? message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error, or <c>null</c>.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c>.</param>
    public ForbiddenException(String? message, Exception? innerException)
        : base(message, innerException)
    { }
}
