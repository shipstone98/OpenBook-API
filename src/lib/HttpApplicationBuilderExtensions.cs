using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Shipstone.AspNetCore.Http;

public static class HttpApplicationBuilderExtensions
{
    public static IApplicationBuilder UseArgumentExceptionHandling(
        this IApplicationBuilder app,
        int statusCode = StatusCodes.Status400BadRequest
    )
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<ArgumentExceptionHandlingMiddleware>();
    }
}