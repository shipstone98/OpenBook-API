using Shipstone.AspNetCore.Http;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Web.Middleware;

#warning Not tested
internal sealed class ForbiddenExceptionHandlingMiddleware
    : ExceptionHandlingMiddleware<ForbiddenException>
{
    internal ForbiddenExceptionHandlingMiddleware(int statusCode)
        : base(statusCode)
    { }
}
