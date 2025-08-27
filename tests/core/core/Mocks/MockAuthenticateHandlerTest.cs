using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Authentication;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockAuthenticationService : IAuthenticationService
{
    internal Action<UserEntity, DateTime> _generateOtpAction;

    public MockAuthenticationService() =>
        this._generateOtpAction = (_, _) =>
            throw new NotImplementedException();

    Task IAuthenticationService.GenerateOtpAsync(
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        this._generateOtpAction(user, now);
        return Task.CompletedTask;
    }
}
