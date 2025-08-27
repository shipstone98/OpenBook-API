using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Shipstone.Extensions.Security;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.MySql;

/// <summary>
/// Provides a set of <c>static</c> (<c>Shared</c> in Visual Basic) methods for registering services with objects that implement <see cref="IServiceCollection" />.
/// </summary>
public static class MySqlDataInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddOpenBookInfrastructureDataMySql(
        this IServiceCollection services,
        String? connectionString = null
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddScoped<IDataSource>(provider =>
                provider.GetRequiredService<MySqlDbContext>())
            .AddScoped(provider =>
            {
                DbContextOptionsBuilder<MySqlDbContext> optionsBuilder = new();

                ServerVersion serverVersion =
                    ServerVersion.AutoDetect(connectionString);

                optionsBuilder.UseMySql(connectionString, serverVersion);

                IEncryptionService encryption =
                    provider.GetRequiredService<IEncryptionService>();

                return new MySqlDbContext(optionsBuilder.Options, encryption);
            });
    }
}
