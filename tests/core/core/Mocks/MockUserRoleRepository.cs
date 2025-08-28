using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockUserRoleRepository : IUserRoleRepository
{
    internal Func<Guid, UserRoleEntity[]> _listForUserFunc;

    internal MockUserRoleRepository() =>
        this._listForUserFunc = _ => throw new NotImplementedException();

    Task<UserRoleEntity[]> IUserRoleRepository.ListForUserAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        UserRoleEntity[] result = this._listForUserFunc(id);
        return Task.FromResult(result);
    }
}
