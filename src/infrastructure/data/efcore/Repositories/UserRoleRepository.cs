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

    Task IUserRoleRepository.CreateAsync(
        UserRoleEntity userRole,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userRole);

        return this._dataSource.UserRoles.SetStateAsync(
            userRole,
            DataEntityState.Created,
            cancellationToken
        );
    }

#warning Not tested
    Task<UserRoleEntity[]> IUserRoleRepository.ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        if (Guid.Equals(userId, Guid.Empty))
        {
            throw new ArgumentException(
                $"{nameof (userId)} is equal to Guid.Empty.",
                nameof (userId)
            );
        }

        return this._dataSource.UserRoles
            .Where(ur => Guid.Equals(userId, ur.UserId))
            .ToArrayAsync(cancellationToken);
    }
}
