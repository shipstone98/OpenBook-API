using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Infrastructure.Authorization;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockAuthorizationService : IAuthorizationService
{
    internal Action<Object, String> _authorizeAction;

    public MockAuthorizationService() =>
        this._authorizeAction = (_, _) => throw new NotImplementedException();

    Task IAuthorizationService.AuthorizeAsync(
        IResource resource,
        String policy,
        CancellationToken cancellationToken
    )
    {
        this._authorizeAction(resource, policy);
        return Task.CompletedTask;
    }
}
