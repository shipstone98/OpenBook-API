using System;
using System.Collections.Generic;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Users;

internal sealed class User : IUser
{
    private readonly IReadOnlySet<String> _roles;
    private readonly UserEntity _user;

    DateTime IUser.Consented => this._user.Consented;
    DateTime IUser.Created => this._user.Created;
    String IUser.EmailAddress => this._user.EmailAddress;
    String IUser.Forename => this._user.Forename;
    Guid IUser.Id => this._user.Id;
    IReadOnlySet<String> IUser.Roles => this._roles;
    String IUser.Surname => this._user.Surname;
    DateTime IUser.Updated => this._user.Updated;
    String IUser.UserName => this._user.UserName;

    internal User(UserEntity user, IReadOnlySet<String> roles)
    {
        this._roles = roles;
        this._user = user;
    }
}
