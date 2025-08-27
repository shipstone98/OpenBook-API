using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockUserRepository : IUserRepository
{
    internal Func<String, UserEntity?> _retrieveFunc;
    internal Action<UserEntity> _updateAction;

    public MockUserRepository()
    {
        this._retrieveFunc = _ => throw new NotImplementedException();
        this._updateAction = _ => throw new NotImplementedException();
    }

    Task<UserEntity?> IUserRepository.RetrieveAsync(
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        UserEntity? result = this._retrieveFunc(emailAddress);
        return Task.FromResult(result);
    }

    Task IUserRepository.UpdateAsync(
        UserEntity user,
        CancellationToken cancellationToken
    )
    {
        this._updateAction(user);
        return Task.CompletedTask;
    }
}
