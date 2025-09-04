using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Web.Authorization;
using Shipstone.OpenBook.Api.Web.Middleware;
using Shipstone.OpenBook.Api.Web.Services;

namespace Shipstone.OpenBook.Api.Web;

/// <summary>
/// Provides a set of <c>static</c> (<c>Shared</c> in Visual Basic) methods for registering services with objects that implement <see cref="IServiceCollection" />.
/// </summary>
public static class WebServiceCollectionExtensions
{
    /// <summary>
    /// Registers OpenBook web authorization services with the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to register services with.</param>
    /// <returns>A reference to <c><paramref name="services" /></c> that can be further used to register services.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="services" /></c> is <c>null</c>.</exception>
    public static IServiceCollection AddOpenBookWebAuthorization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddAuthorization(options =>
            {
                options.AddPolicy(
                    Policies.ResourceOwner,
                    policy =>
                    {
                        IAuthorizationRequirement requirement =
                            new ResourceOwnerAuthorizationRequirement();

                        policy.AddRequirements(requirement);
                    }
                );
            })
            .AddScoped<IAuthorizationHandler, ResourceOwnerAuthorizationHandler>();
    }

    /// <summary>
    /// Registers OpenBook web claims services with the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to register services with.</param>
    /// <returns>A reference to <c><paramref name="services" /></c> that can be further used to register services.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="services" /></c> is <c>null</c>.</exception>
    public static IServiceCollection AddOpenBookWebClaims(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddScoped<IClaimsService>(provider =>
                provider.GetRequiredService<ClaimsService>())
            .AddScoped<ClaimsService>()
            .AddScoped<ClaimsMiddleware>();
    }

    /// <summary>
    /// Registers OpenBook web <see cref="ForbiddenException" /> handling services with the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to register services with.</param>
    /// <param name="statusCode">The HTTP status code to return when an instance of <see cref="NotFoundException" /> is thrown.</param>
    /// <returns>A reference to <c><paramref name="services" /></c> that can be further used to register services.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="services" /></c> is <c>null</c>.</exception>
    public static IServiceCollection AddOpenBookWebForbiddenExceptionHandling(
        this IServiceCollection services,
        int statusCode = StatusCodes.Status403Forbidden
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddSingleton(_ =>
            new ForbiddenExceptionHandlingMiddleware(statusCode));
    }

    /// <summary>
    /// Registers OpenBook web <see cref="NotFoundException" /> handling services with the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to register services with.</param>
    /// <param name="statusCode">The HTTP status code to return when an instance of <see cref="NotFoundException" /> is thrown.</param>
    /// <returns>A reference to <c><paramref name="services" /></c> that can be further used to register services.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="services" /></c> is <c>null</c>.</exception>
    public static IServiceCollection AddOpenBookWebNotFoundExceptionHandling(
        this IServiceCollection services,
        int statusCode = StatusCodes.Status404NotFound
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddSingleton(_ =>
            new NotFoundExceptionHandlingMiddleware(statusCode));
    }
}
