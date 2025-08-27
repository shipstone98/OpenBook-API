using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shipstone.Extensions.Identity;

public static class IdentityServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityExtensions(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services.AddSingleton<IPasswordService, PasswordService>();
    }
}
