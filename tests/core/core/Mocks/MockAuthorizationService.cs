using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Authorization;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockAuthorizationService : IAuthorizationService
{
    internal Action<Object, String> _authorizeAction;

    public MockAuthorizationService() =>
        this._authorizeAction = (_, _) => throw new NotImplementedException();

    Task IAuthorizationService.AuthorizeAsync<TId>(
        CreatableEntity<TId> entity,
        String policy,
        CancellationToken cancellationToken
    )
    {
        this._authorizeAction(entity, policy);
        return Task.CompletedTask;
    }
}
