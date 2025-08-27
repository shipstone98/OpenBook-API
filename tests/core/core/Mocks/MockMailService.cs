using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Mail;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockMailService : IMailService
{
    internal Action<UserEntity, int> _sendOtpAction;

    public MockMailService() =>
        this._sendOtpAction = (_, _) => throw new NotImplementedException();

    Task IMailService.SendOtpAsync(
        UserEntity user,
        int expiryMinutes,
        CancellationToken cancellationToken
    )
    {
        this._sendOtpAction(user, expiryMinutes);
        return Task.CompletedTask;
    }
}
