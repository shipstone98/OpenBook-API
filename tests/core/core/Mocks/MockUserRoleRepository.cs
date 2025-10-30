using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockUserRoleRepository : IUserRoleRepository
{
    internal Action<UserRoleEntity> _createAction;
    internal Action<UserRoleEntity> _deleteAction;
    internal Func<Guid, UserRoleEntity[]> _listForUserFunc;

    internal MockUserRoleRepository()
    {
        this._createAction = _ => { };
        this._deleteAction = _ => { };
        this._listForUserFunc = _ => throw new NotImplementedException();
    }

    Task IUserRoleRepository.CreateAsync(
        UserRoleEntity userRole,
        CancellationToken cancellationToken
    )
    {
        this._createAction(userRole);
        return Task.CompletedTask;
    }

    Task IUserRoleRepository.DeleteAsync(
        UserRoleEntity userRole,
        CancellationToken cancellationToken
    )
    {
        this._deleteAction(userRole);
        return Task.CompletedTask;
    }

    Task<UserRoleEntity[]> IUserRoleRepository.ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        UserRoleEntity[] result = this._listForUserFunc(userId);
        return Task.FromResult(result);
    }
}
