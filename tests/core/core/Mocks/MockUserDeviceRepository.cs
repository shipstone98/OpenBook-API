using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockUserDeviceRepository : IUserDeviceRepository
{
    internal Func<Guid, UserDeviceEntity[]> _listForUserFunc;

    internal MockUserDeviceRepository() =>
        this._listForUserFunc = _ => throw new NotImplementedException();

    Task<UserDeviceEntity[]> IUserDeviceRepository.ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        UserDeviceEntity[] userDevices = this._listForUserFunc(userId);
        return Task.FromResult(userDevices);
    }
}
