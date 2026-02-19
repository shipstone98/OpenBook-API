using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Shipstone.Utilities.Linq;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Web.Services;

internal sealed class ClaimsService : IClaimsService
{
    private String? _emailAddress;
    private Nullable<Guid> _id;
    private bool _isAuthenticated;
    private IReadOnlySet<String>? _roles;
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
    IReadOnlySet<String> IClaimsService.Roles => this.Get(() => this._roles);
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

        IEnumerable<Claim> roleClaims =
            claims.Where(c => c.Type.Equals(ClaimTypes.Role));

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

            this._roles =
                roleClaims
                    .Select(c => c.Value)
                    .ToSortedSet();

            this._userName = userNameClaim.Value;
        }
    }

    private T Get<T>(Func<T?> func) where T : class
    {
        T? property = func();

        if (property is null)
        {
            throw new UnauthorizedException("The current user is not authenticated.");
        }

        return property;
    }
}
