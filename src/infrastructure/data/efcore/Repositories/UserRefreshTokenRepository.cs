using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

    Task IUserRefreshTokenRepository.DeleteAsync(
        UserRefreshTokenEntity userRefreshToken,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userRefreshToken);

        return this._dataSource.UserRefreshTokens.SetStateAsync(
            userRefreshToken,
            DataEntityState.Deleted,
            cancellationToken
        );
    }

    Task<UserRefreshTokenEntity?> IUserRefreshTokenRepository.RetrieveAsync(
        String val,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(val);

        return this._dataSource.UserRefreshTokens.FirstOrDefaultAsync(
            urt => String.Equals(val, urt.Value),
            cancellationToken
        );
    }
}
