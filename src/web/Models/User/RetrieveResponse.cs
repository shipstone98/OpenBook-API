using System;
using System.Collections.Generic;

using Shipstone.OpenBook.Api.Core.Users;

namespace Shipstone.OpenBook.Api.Web.Models.User;

internal sealed class RetrieveResponse
{
    private readonly IUser _user;

    public DateTime Consented => this._user.Consented;
    public DateTime Created => this._user.Created;
    public Guid Id => this._user.Id;
    public IEnumerable<String> Roles => this._user.Roles;
    public DateTime Updated => this._user.Updated;
    public String UserName => this._user.UserName;

    internal RetrieveResponse(IUser user) => this._user = user;
}
