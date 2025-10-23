using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

    Task IUserFollowingRepository.DeleteAsync(
        UserFollowingEntity userFollowing,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userFollowing);

        return this._dataSource.UserFollowings.SetStateAsync(
            userFollowing,
            DataEntityState.Deleted,
            cancellationToken
        );
    }

#warning Not tested
    Task<UserFollowingEntity[]> IUserFollowingRepository.ListForFolloweeAsync(
        Guid followeeId,
        CancellationToken cancellationToken
    )
    {
        if (Guid.Equals(followeeId, Guid.Empty))
        {
            throw new ArgumentException(
                $"{nameof (followeeId)} is equal to Guid.Empty.",
                nameof (followeeId)
            );
        }

        return this._dataSource.UserFollowings
            .Where(uf => Guid.Equals(followeeId, uf.FolloweeId))
            .ToArrayAsync(cancellationToken);
    }

#warning Not tested
    Task<UserFollowingEntity[]> IUserFollowingRepository.ListForFollowerAsync(
        Guid followerId,
        CancellationToken cancellationToken
    )
    {
        if (Guid.Equals(followerId, Guid.Empty))
        {
            throw new ArgumentException(
                $"{nameof (followerId)} is equal to Guid.Empty.",
                nameof (followerId)
            );
        }

        return this._dataSource.UserFollowings
            .Where(uf => Guid.Equals(followerId, uf.FollowerId))
            .ToArrayAsync(cancellationToken);
    }

    Task<UserFollowingEntity?> IUserFollowingRepository.RetrieveAsync(
        Guid followerId,
        Guid followeeId,
        CancellationToken cancellationToken
    )
    {
        if (Guid.Equals(followerId, Guid.Empty))
        {
            throw new ArgumentException(
                $"{nameof (followerId)} is equal to Guid.Empty.",
                nameof (followerId)
            );
        }

        if (Guid.Equals(followeeId, Guid.Empty))
        {
            throw new ArgumentException(
                $"{nameof (followeeId)} is equal to Guid.Empty.",
                nameof (followeeId)
            );
        }

        return this._dataSource.UserFollowings.FirstOrDefaultAsync(
            uf =>
                Guid.Equals(followerId, uf.FollowerId)
                && Guid.Equals(followeeId, uf.FolloweeId),
            cancellationToken
        );
    }
}
