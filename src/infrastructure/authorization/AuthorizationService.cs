using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Infrastructure.Authorization;

internal sealed class AuthorizationService : IAuthorizationService
{
    private readonly Microsoft.AspNetCore.Authorization.IAuthorizationService _authorization;

    public AuthorizationService(Microsoft.AspNetCore.Authorization.IAuthorizationService authorization)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        this._authorization = authorization;
    }

    private async Task AuthorizeAsync(
        IResource resource,
        String policy
    )
    {
        ClaimsPrincipal user = new();

        AuthorizationResult result =
            await this._authorization.AuthorizeAsync(user, resource, policy);

        if (!result.Succeeded)
        {
            throw new ForbiddenException("The current user is not authorized to modify the provided entity.");
        }
    }

    Task IAuthorizationService.AuthorizeAsync(
        IResource resource,
        String policy,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(policy);
        return this.AuthorizeAsync(resource, policy);
    }
}
