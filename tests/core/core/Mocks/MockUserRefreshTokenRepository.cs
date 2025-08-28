using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockUserRefreshTokenRepository
    : IUserRefreshTokenRepository
{
    internal Action<UserRefreshTokenEntity> _createAction;

    internal MockUserRefreshTokenRepository() =>
        this._createAction = _ => throw new NotImplementedException();

    Task IUserRefreshTokenRepository.CreateAsync(
        UserRefreshTokenEntity userRefreshToken,
        CancellationToken cancellationToken
    )
    {
        this._createAction(userRefreshToken);
        return Task.CompletedTask;
    }
}
