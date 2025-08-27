using System;

namespace Shipstone.AspNetCore.Http;

internal sealed class ArgumentExceptionHandlingMiddleware
    : ExceptionHandlingMiddleware<ArgumentException>
{
    internal ArgumentExceptionHandlingMiddleware(int statusCode)
        : base(statusCode)
    { }
}
