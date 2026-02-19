using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Web.Authorization;

internal sealed class AdministratorAuthorizationHandler
    : AuthorizationHandler<AdministratorAuthorizationRequirement, IResource>
{
    private readonly IClaimsService _claims;

    public AdministratorAuthorizationHandler(IClaimsService claims)
    {
        ArgumentNullException.ThrowIfNull(claims);
        this._claims = claims;
    }

    protected sealed override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdministratorAuthorizationRequirement requirement,
        IResource resource
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);
        ArgumentNullException.ThrowIfNull(resource);

        if (this._claims.IsAuthenticated)
        {
            IReadOnlySet<String> roles = this._claims.Roles;
            IReadOnlySet<String> creatorRoles = resource.CreatorRoles;

            if (
                !creatorRoles.Contains(Roles.SystemAdministrator)
                && (
                    roles.Contains(Roles.SystemAdministrator)
                    || (
                        !creatorRoles.Contains(Roles.Administrator)
                        && roles.Contains(Roles.Administrator)
                    )
                )
            )
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
