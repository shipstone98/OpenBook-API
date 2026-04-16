using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

using Shipstone.Utilities.Linq;

using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Web.Services;

namespace Shipstone.OpenBook.Api.Web.Authentication;

internal sealed class OpenBookClaimsTransformation : IClaimsTransformation
{
    private readonly ClaimsService _claims;
    private readonly IUserRetrieveHandler _handler;

    public OpenBookClaimsTransformation(
        IUserRetrieveHandler handler,
        ClaimsService claims
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(claims);
        this._claims = claims;
        this._handler = handler;
    }

    private async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        IEnumerable<Claim> claims = await this.TransformClaimsAsync(principal);
        ClaimsIdentity identity = new(claims);
        principal.AddIdentity(identity);
        return principal;
    }

    private async Task<IEnumerable<Claim>> TransformClaimsAsync(ClaimsPrincipal principal)
    {
        Claim? nameIdClaim =
            principal.Claims.FirstOrDefault(c =>
                c.Type.Equals(ClaimTypes.NameIdentifier));

        if (!Guid.TryParse(nameIdClaim?.Value, out Guid identityId))
        {
            return Array.Empty<Claim>();
        }

        IUser user;

        try
        {
            user =
                await this._handler.HandleAsync(
                    identityId,
                    CancellationToken.None
                );
        }

        catch
        {
            return Array.Empty<Claim>();
        }

        this._claims._user = user;
        SortedSet<String> roles = user.Roles.ToSortedSet();

        IEnumerable<String> claimedRoles =
            principal.Claims
                .Where(c => c.Type.Equals(ClaimTypes.Role))
                .Select(c => c.Value);

        roles.ExceptWith(claimedRoles);
        return roles.Select(r => new Claim(ClaimTypes.Role, r));
    }

    Task<ClaimsPrincipal> IClaimsTransformation.TransformAsync(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);
        return this.TransformAsync(principal);
    }
}
