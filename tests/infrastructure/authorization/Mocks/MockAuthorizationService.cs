using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Shipstone.OpenBook.Api.Infrastructure.AuthorizationTest.Mocks;

internal sealed class MockAuthorizationService : IAuthorizationService
{
    internal Func<ClaimsPrincipal, Object?, String, AuthorizationResult> _authorizeFunc;

    public MockAuthorizationService() =>
        this._authorizeFunc = (_, _, _) => throw new NotImplementedException();

    Task<AuthorizationResult> IAuthorizationService.AuthorizeAsync(
        ClaimsPrincipal user,
        Object? resource,
        String policyName
    )
    {
        AuthorizationResult result =
            this._authorizeFunc(user, resource, policyName);

        return Task.FromResult(result);
    }

    Task<AuthorizationResult> IAuthorizationService.AuthorizeAsync(
        ClaimsPrincipal user,
        Object? resource,
        IEnumerable<IAuthorizationRequirement> requirements
    ) =>
        throw new NotImplementedException();
}
