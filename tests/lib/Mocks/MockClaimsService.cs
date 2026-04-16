using System;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Users;

namespace Shipstone.OpenBook.Api.Test.Mocks;

public sealed class MockClaimsService : IClaimsService
{
    public Func<bool> _isAuthenticatedFunc;
    public Func<IUser> _userFunc;

    bool IClaimsService.IsAuthenticated => this._isAuthenticatedFunc();
    IUser IClaimsService.User => this._userFunc();

    public MockClaimsService()
    {
        this._isAuthenticatedFunc = () => throw new NotImplementedException();
        this._userFunc = () => throw new NotImplementedException();
    }
}
