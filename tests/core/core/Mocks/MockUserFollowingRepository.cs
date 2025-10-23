using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockUserFollowingRepository : IUserFollowingRepository
{
    internal Action<UserFollowingEntity> _createAction;
    internal Action<UserFollowingEntity> _deleteAction;
    internal Func<Guid, UserFollowingEntity[]> _listForFolloweeFunc;
    internal Func<Guid, UserFollowingEntity[]> _listForFollowerFunc;
    internal Func<Guid, Guid, UserFollowingEntity?> _retrieveFunc;

    internal MockUserFollowingRepository()
    {
        this._createAction = _ => throw new NotImplementedException();
        this._deleteAction = _ => throw new NotImplementedException();
        this._listForFolloweeFunc = _ => throw new NotImplementedException();
        this._listForFollowerFunc = _ => throw new NotImplementedException();
        this._retrieveFunc = (_, _) => throw new NotImplementedException();
    }

    Task IUserFollowingRepository.CreateAsync(
        UserFollowingEntity userFollowing,
        CancellationToken cancellationToken
    )
    {
        this._createAction(userFollowing);
        return Task.CompletedTask;
    }

    Task IUserFollowingRepository.DeleteAsync(
        UserFollowingEntity userFollowing,
        CancellationToken cancellationToken
    )
    {
        this._deleteAction(userFollowing);
        return Task.CompletedTask;
    }

    Task<UserFollowingEntity[]> IUserFollowingRepository.ListForFolloweeAsync(
        Guid followeeId,
        CancellationToken cancellationToken
    )
    {
        UserFollowingEntity[] result = this._listForFolloweeFunc(followeeId);
        return Task.FromResult(result);
    }

    Task<UserFollowingEntity[]> IUserFollowingRepository.ListForFollowerAsync(
        Guid followerId,
        CancellationToken cancellationToken
    )
    {
        UserFollowingEntity[] result = this._listForFollowerFunc(followerId);
        return Task.FromResult(result);
    }

    Task<UserFollowingEntity?> IUserFollowingRepository.RetrieveAsync(
        Guid followerId,
        Guid followeeId,
        CancellationToken cancellationToken
    )
    {
        UserFollowingEntity? userFollowing =
            this._retrieveFunc(followerId, followeeId);

        return Task.FromResult(userFollowing);
    }
}
