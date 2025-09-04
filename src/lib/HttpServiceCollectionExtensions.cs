using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Shipstone.Extensions.Pagination;

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

    public static IServiceCollection AddNcsaCommonLogging(
        this IServiceCollection services,
        TextWriter writer
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(writer);

        return services.AddSingleton(_ =>
            new NcsaCommonLoggingMiddleware(writer));
    }

    public static IServiceCollection AddPagination(
        this IServiceCollection services,
        Action<PaginationOptions>? configurePagination = null
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddScoped<PaginationMiddleware>()
            .Configure<PaginationOptions>(options =>
            {
                if (configurePagination is not null)
                {
                    configurePagination(options);
                }
            });
    }
}