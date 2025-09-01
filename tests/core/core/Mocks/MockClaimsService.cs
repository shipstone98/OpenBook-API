using System;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockClaimsService : IClaimsService
{
    internal Func<String> _emailAddressFunc;
    internal Func<Guid> _idFunc;
    internal Func<bool> _isAuthenticatedFunc;
    internal Func<String> _userNameFunc;

    String IClaimsService.EmailAddress => this._emailAddressFunc();
    Guid IClaimsService.Id => this._idFunc();
    bool IClaimsService.IsAuthenticated => this._isAuthenticatedFunc();
    String IClaimsService.UserName => this._userNameFunc();

    public MockClaimsService()
    {
        this._emailAddressFunc = () => throw new NotImplementedException();
        this._idFunc = () => throw new NotImplementedException();
        this._isAuthenticatedFunc = () => throw new NotImplementedException();
        this._userNameFunc = () => throw new NotImplementedException();
    }
}
