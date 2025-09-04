using System;
using Microsoft.AspNetCore.Builder;

namespace Shipstone.AspNetCore.Http;

public static class HttpApplicationBuilderExtensions
{
    public static IApplicationBuilder UseArgumentExceptionHandling(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<ArgumentExceptionHandlingMiddleware>();
    }

    public static IApplicationBuilder UsePagination(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<PaginationMiddleware>();
    }
}