using System;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockAuthenticateResult : IAuthenticateResult
{
    internal Func<String> _accessTokenFunc;
    internal Func<String> _refreshTokenFunc;
    internal Func<DateTime> _refreshTokenExpiresFunc;

    String IAuthenticateResult.AccessToken => this._accessTokenFunc();
    String IAuthenticateResult.RefreshToken => this._refreshTokenFunc();

    DateTime IAuthenticateResult.RefreshTokenExpires =>
        this._refreshTokenExpiresFunc();

    internal MockAuthenticateResult()
    {
        this._accessTokenFunc = () => throw new NotImplementedException();
        this._refreshTokenFunc = () => throw new NotImplementedException();

        this._refreshTokenExpiresFunc = () =>
            throw new NotImplementedException();
    }
}
