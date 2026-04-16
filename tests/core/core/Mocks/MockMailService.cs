using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Mail;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockMailService : IMailService
{
    internal Action _sendRegistrationAction;
    internal Action _sendUnregistrationAction;

    public MockMailService()
    {
        this._sendRegistrationAction = () =>
            throw new NotImplementedException();

        this._sendUnregistrationAction = () =>
            throw new NotImplementedException();
    }

    Task IMailService.SendRegistrationAsync(CancellationToken cancellationToken)
    {
        this._sendRegistrationAction();
        return Task.CompletedTask;
    }

    Task IMailService.SendUnregistrationAsync(CancellationToken cancellationToken)
    {
        this._sendUnregistrationAction();
        return Task.CompletedTask;
    }
}
