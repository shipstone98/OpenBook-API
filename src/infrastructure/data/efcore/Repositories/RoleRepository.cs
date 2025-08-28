using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Repositories;

internal sealed class RoleRepository : IRoleRepository
{
    private readonly IDataSource _dataSource;

    public RoleRepository(IDataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        this._dataSource = dataSource;
    }

    Task<RoleEntity?> IRoleRepository.RetrieveAsync(
        long id,
        CancellationToken cancellationToken
    )
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);

        return this._dataSource.Roles.FirstOrDefaultAsync(
            r => id == r.Id,
            cancellationToken
        );
    }
}
