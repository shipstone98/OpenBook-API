using System;
using System.Collections.Generic;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Test.Mocks;

public sealed class MockClaimsService : IClaimsService
{
    public Func<String> _emailAddressFunc;
    public Func<Guid> _idFunc;
    public Func<bool> _isAuthenticatedFunc;
    public Func<IReadOnlySet<String>> _rolesFunc;
    public Func<String> _userNameFunc;

    String IClaimsService.EmailAddress => this._emailAddressFunc();
    Guid IClaimsService.Id => this._idFunc();
    bool IClaimsService.IsAuthenticated => this._isAuthenticatedFunc();
    IReadOnlySet<String> IClaimsService.Roles => this._rolesFunc();
    String IClaimsService.UserName => this._userNameFunc();

    public MockClaimsService()
    {
        this._emailAddressFunc = () => throw new NotImplementedException();
        this._idFunc = () => throw new NotImplementedException();
        this._isAuthenticatedFunc = () => throw new NotImplementedException();
        this._rolesFunc = () => throw new NotImplementedException();
        this._userNameFunc = () => throw new NotImplementedException();
    }
}
