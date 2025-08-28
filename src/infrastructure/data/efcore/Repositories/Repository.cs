using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Repositories;

internal sealed class Repository : IRepository
{
    private readonly IDataSource _dataSource;
    private readonly IServiceProvider _provider;

    IRoleRepository IRepository.Roles =>
        this._provider.GetRequiredService<IRoleRepository>();

    IUserRefreshTokenRepository IRepository.UserRefreshTokens =>
        this._provider.GetRequiredService<IUserRefreshTokenRepository>();

    IUserRoleRepository IRepository.UserRoles =>
        this._provider.GetRequiredService<IUserRoleRepository>();

    IUserRepository IRepository.Users =>
        this._provider.GetRequiredService<IUserRepository>();

    public Repository(IServiceProvider provider, IDataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(dataSource);
        this._dataSource = dataSource;
        this._provider = provider;
    }

    Task IRepository.SaveAsync(CancellationToken cancellationToken) =>
        this._dataSource.SaveAsync(cancellationToken);
}
