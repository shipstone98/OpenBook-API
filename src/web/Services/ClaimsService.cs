using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Web.Services;

internal sealed class ClaimsService : IClaimsService
{
    private Nullable<Guid> _id;

    Guid IClaimsService.Id
    {
        get
        {
            if (!this._id.HasValue)
            {
                throw new UnauthorizedException("The current user is not authenticated.");
            }

            return this._id.Value;
        }
    }

    internal void Authenticate(IEnumerable<Claim> claims)
    {
        Claim? claim =
            claims.FirstOrDefault(c =>
                c.Type.Equals(ClaimTypes.NameIdentifier));

        if (claim is null || !Guid.TryParse(claim.Value, out Guid id))
        {
            return;
        }

        this._id = id;
    }
}
