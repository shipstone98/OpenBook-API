using Shipstone.AspNetCore.Http;

using Shipstone.OpenBook.Api.Core;

namespace Shipstone.OpenBook.Api.Web.Middleware;

#warning Not tested
internal sealed class ConflictExceptionHandlingMiddleware
    : ExceptionHandlingMiddleware<ConflictException>
{
    internal ConflictExceptionHandlingMiddleware(int statusCode)
        : base(statusCode)
    { }
}
