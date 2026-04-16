using System;

namespace Shipstone.OpenBook.Api.Web.Models.Account;

internal sealed class RegisterRequest
{
#pragma warning disable CS8618
    internal String _userName;
#pragma warning restore CS8618

    public String UserName
    {
        set => this._userName = value;
    }
}
