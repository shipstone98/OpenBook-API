using System;

namespace Shipstone.Extensions.Identity;

public class PasswordNotValidException : ArgumentException
{
    private readonly PasswordRequirements _failedRequirements;

    public PasswordRequirements FailedRequirements => this._failedRequirements;

    public PasswordNotValidException() { }

    public PasswordNotValidException(String? message) : base(message) { }

    public PasswordNotValidException(
        String? message,
        PasswordRequirements failedRequirements,
        Exception? innerException
    )
        : base(message, innerException) =>
            this._failedRequirements = failedRequirements;

    public PasswordNotValidException(
        String? message,
        Exception? innerException
    )
        : base(message, innerException)
    { }

    public PasswordNotValidException(
        String? paramName,
        PasswordRequirements failedRequirements,
        String? message
    )
        : base(message, paramName) =>
            this._failedRequirements = failedRequirements;

    public PasswordNotValidException(String? paramName, String? message)
        : base(message, paramName)
    { }
}
