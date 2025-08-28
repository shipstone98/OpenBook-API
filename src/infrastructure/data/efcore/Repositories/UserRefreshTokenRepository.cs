using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Repositories;

internal sealed class UserRefreshTokenRepository : IUserRefreshTokenRepository
{
    private readonly IDataSource _dataSource;

    public UserRefreshTokenRepository(IDataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        this._dataSource = dataSource;
    }

    Task IUserRefreshTokenRepository.CreateAsync(
        UserRefreshTokenEntity userRefreshToken,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userRefreshToken);

        return this._dataSource.UserRefreshTokens.SetStateAsync(
            userRefreshToken,
            DataEntityState.Created,
            cancellationToken
        );
    }
}
