using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Web.Authorization;

internal sealed class ResourceOwnerAuthorizationHandler
    : AuthorizationHandler<ResourceOwnerAuthorizationRequirement, IResource>
{
    private readonly IClaimsService _claims;

    public ResourceOwnerAuthorizationHandler(IClaimsService claims)
    {
        ArgumentNullException.ThrowIfNull(claims);
        this._claims = claims;
    }

    protected sealed override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOwnerAuthorizationRequirement requirement,
        IResource resource
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        if (
            this._claims.IsAuthenticated
            && Guid.Equals(this._claims.Id, resource.CreatorId)
        )
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
