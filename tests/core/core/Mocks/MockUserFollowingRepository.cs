using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockUserFollowingRepository : IUserFollowingRepository
{
    internal Action<UserFollowingEntity> _createAction;

    internal MockUserFollowingRepository() =>
        this._createAction = _ => throw new NotImplementedException();

    Task IUserFollowingRepository.CreateAsync(
        UserFollowingEntity userFollowing,
        CancellationToken cancellationToken
    )
    {
        this._createAction(userFollowing);
        return Task.CompletedTask;
    }
}
