using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Repositories;

internal sealed class UserRoleRepository : IUserRoleRepository
{
    private readonly IDataSource _dataSource;

    public UserRoleRepository(IDataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        this._dataSource = dataSource;
    }

#warning Not tested
    Task<UserRoleEntity[]> IUserRoleRepository.ListForUserAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        if (Guid.Equals(id, Guid.Empty))
        {
            throw new ArgumentException(
                $"{nameof (id)} is equal to Guid.Empty.",
                nameof (id)
            );
        }

        return this._dataSource.UserRoles
            .Where(ur => Guid.Equals(id, ur.UserId))
            .ToArrayAsync(cancellationToken);
    }
}
