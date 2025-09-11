using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockUserFollowingRepository : IUserFollowingRepository
{
    internal Action<UserFollowingEntity> _createAction;
    internal Func<Guid, UserFollowingEntity[]> _listForFolloweeFunc;

    internal MockUserFollowingRepository()
    {
        this._createAction = _ => throw new NotImplementedException();
        this._listForFolloweeFunc = _ => throw new NotImplementedException();
    }

    Task IUserFollowingRepository.CreateAsync(
        UserFollowingEntity userFollowing,
        CancellationToken cancellationToken
    )
    {
        this._createAction(userFollowing);
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
}
