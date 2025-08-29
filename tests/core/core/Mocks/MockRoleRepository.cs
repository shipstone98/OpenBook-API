using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockRoleRepository : IRoleRepository
{
    internal Func<long, RoleEntity?> _retrieveFunc;

    internal MockRoleRepository() =>
        this._retrieveFunc = _ => throw new NotImplementedException();

    Task<RoleEntity?> IRoleRepository.RetrieveAsync(
        long id,
        CancellationToken cancellationToken
    )
    {
        RoleEntity? result = this._retrieveFunc(id);
        return Task.FromResult(result);
    }
}
