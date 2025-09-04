using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Authorization;

internal sealed class AuthorizationService : IAuthorizationService
{
    private readonly Microsoft.AspNetCore.Authorization.IAuthorizationService _authorization;

    public AuthorizationService(Microsoft.AspNetCore.Authorization.IAuthorizationService authorization)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        this._authorization = authorization;
    }

    private async Task AuthorizeAsync<TId>(
        CreatableEntity<TId> entity,
        String policy
    )
        where TId : struct
    {
        ClaimsPrincipal user = new();

        AuthorizationResult result =
            await this._authorization.AuthorizeAsync(user, entity, policy);

        if (!result.Succeeded)
        {
            throw new ForbiddenException("The current user is not authorized to modify the provided entity.");
        }
    }

    Task IAuthorizationService.AuthorizeAsync<TId>(
        CreatableEntity<TId> entity,
        String policy,
        CancellationToken cancellationToken
    )
        where TId : struct
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(policy);
        return this.AuthorizeAsync(entity, policy);
    }
}
