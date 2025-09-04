using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Repositories;

internal sealed class UserFollowingRepository : IUserFollowingRepository
{
    private readonly IDataSource _dataSource;

    public UserFollowingRepository(IDataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        this._dataSource = dataSource;
    }

    Task IUserFollowingRepository.CreateAsync(
        UserFollowingEntity userFollowing,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userFollowing);

        return this._dataSource.UserFollowings.SetStateAsync(
            userFollowing,
            DataEntityState.Created,
            cancellationToken
        );
    }
}
