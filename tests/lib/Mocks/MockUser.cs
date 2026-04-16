using System;
using System.Collections.Generic;

using Shipstone.OpenBook.Api.Core.Users;

namespace Shipstone.OpenBook.Api.Test.Mocks;

public sealed class MockUser : IUser
{
    public Func<Guid> _idFunc;
    public Func<IReadOnlySet<String>> _rolesFunc;
    public Func<String> _userNameFunc;

    DateTime IUser.Consented => throw new NotImplementedException();
    DateTime IUser.Created => throw new NotImplementedException();
    Guid IUser.Id => this._idFunc();
    IReadOnlySet<String> IUser.Roles => this._rolesFunc();
    DateTime IUser.Updated => throw new NotImplementedException();
    String IUser.UserName => this._userNameFunc();

    public MockUser()
    {
        this._idFunc = () => throw new NotImplementedException();
        this._rolesFunc = () => throw new NotImplementedException();
        this._userNameFunc = () => throw new NotImplementedException();
    }
}
