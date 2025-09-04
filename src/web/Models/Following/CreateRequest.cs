using System;

namespace Shipstone.OpenBook.Api.Web.Models.Following;

internal sealed class CreateRequest
{
#pragma warning disable CS8618
    internal String _userName;
#pragma warning restore CS8618

    public String UserName
    {
        set => this._userName = value;
    }
}
