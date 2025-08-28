using System;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Web.Models.Account;

internal sealed class OtpAuthenticateResponse
{
    private readonly String _accessToken;
    private readonly String _refreshToken;

    public String AccessToken => this._accessToken;
    public String RefreshToken => this._refreshToken;

    internal OtpAuthenticateResponse(IAuthenticateResult result)
    {
        this._accessToken = result.AccessToken;
        this._refreshToken = result.RefreshToken;
    }
}
