using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockUserDeviceRepository : IUserDeviceRepository
{
    internal Action<UserDeviceEntity> _deleteAction;
    internal Func<Guid, UserDeviceEntity[]> _listForUserFunc;

    internal MockUserDeviceRepository()
    {
        this._deleteAction = _ => throw new NotImplementedException();
        this._listForUserFunc = _ => throw new NotImplementedException();
    }

    Task IUserDeviceRepository.DeleteAsync(
        UserDeviceEntity userDevice,
        CancellationToken cancellationToken
    )
    {
        this._deleteAction(userDevice);
        return Task.CompletedTask;
    }

    Task<UserDeviceEntity[]> IUserDeviceRepository.ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        UserDeviceEntity[] userDevices = this._listForUserFunc(userId);
        return Task.FromResult(userDevices);
    }
}
