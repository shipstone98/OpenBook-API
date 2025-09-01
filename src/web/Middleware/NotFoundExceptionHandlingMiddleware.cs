using Shipstone.AspNetCore.Http;

using Shipstone.OpenBook.Api.Core;

namespace Shipstone.OpenBook.Api.Web.Middleware;

#warning Not tested
internal sealed class NotFoundExceptionHandlingMiddleware
    : ExceptionHandlingMiddleware<NotFoundException>
{
    internal NotFoundExceptionHandlingMiddleware(int statusCode)
        : base(statusCode)
    { }
}
