using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shipstone.OpenBook.Api.Infrastructure.Authorization;

/// <summary>
/// Provides a set of <c>static</c> (<c>Shared</c> in Visual Basic) methods for registering services with objects that implement <see cref="IServiceCollection" />.
/// </summary>
public static class AuthorizationInfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Registers OpenBook authorization infrastructure services with the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to register services with.</param>
    /// <returns>A reference to <c><paramref name="services" /></c> that can be further used to register services.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="services" /></c> is <c>null</c>.</exception>
    public static IServiceCollection AddOpenBookInfrastructureAuthorization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services.AddScoped<IAuthorizationService, AuthorizationService>();
    }
}
