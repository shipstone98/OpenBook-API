using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Web.Services;

internal sealed class ClaimsService : IClaimsService
{
    private String? _emailAddress;
    private Nullable<Guid> _id;
    private bool _isAuthenticated;
    private String? _userName;

    String IClaimsService.EmailAddress => this.Get(() => this._emailAddress);

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

    bool IClaimsService.IsAuthenticated => this._isAuthenticated;
    String IClaimsService.UserName => this.Get(() => this._userName);

    internal void Authenticate(IEnumerable<Claim> claims)
    {
        Claim? emailAddressClaim =
            claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email));

        Claim? idClaim =
            claims.FirstOrDefault(c =>
                c.Type.Equals(ClaimTypes.NameIdentifier));

        Claim? userNameClaim =
            claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name));

        if (
            emailAddressClaim is null
            || idClaim is null
            || userNameClaim is null
            || !Guid.TryParse(idClaim.Value, out Guid id)
        )
        {
            this._emailAddress = null;
            this._id = null;
            this._isAuthenticated = false;
            this._userName = null;
        }

        else
        {
            this._emailAddress = emailAddressClaim.Value;
            this._id = id;
            this._isAuthenticated = true;
            this._userName = userNameClaim.Value;
        }
    }

    private String Get(Func<String?> func)
    {
        String? property = func();

        if (property is null)
        {
            throw new UnauthorizedException("The current user is not authenticated.");
        }

        return property;
    }
}
