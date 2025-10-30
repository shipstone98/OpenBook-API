using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Repositories;

internal sealed class UserDeviceRepository : IUserDeviceRepository
{
    private readonly IDataSource _dataSource;

    public UserDeviceRepository(IDataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        this._dataSource = dataSource;
    }

    Task IUserDeviceRepository.DeleteAsync(
        UserDeviceEntity userDevice,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userDevice);

        return this._dataSource.UserDevices.SetStateAsync(
            userDevice,
            DataEntityState.Deleted,
            cancellationToken
        );
    }

#warning Not tested
    Task<UserDeviceEntity[]> IUserDeviceRepository.ListForUserAsync(
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

        return this._dataSource.UserDevices
            .Where(ud => Guid.Equals(userId, ud.UserId))
            .ToArrayAsync(cancellationToken);
    }
}
