using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Shipstone.AspNetCore.Http;

public static class HttpServiceCollectionExtensions
{
    public static IServiceCollection AddArgumentExceptionHandling(
        this IServiceCollection services,
        int statusCode = StatusCodes.Status400BadRequest
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddSingleton(_ =>
            new ArgumentExceptionHandlingMiddleware(statusCode));
    }
}