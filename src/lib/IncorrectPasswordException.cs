using System;

namespace Shipstone.Extensions.Identity;

public class IncorrectPasswordException : Exception
{
    public IncorrectPasswordException() { }

    public IncorrectPasswordException(String? message) : base(message) { }

    public IncorrectPasswordException(
        String? message,
        Exception? innerException
    )
        : base(message, innerException)
    { }
}
