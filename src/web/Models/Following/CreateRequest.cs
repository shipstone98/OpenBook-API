using System;

namespace Shipstone.OpenBook.Api.Web.Models.Following;

internal sealed class CreateRequest
{
#pragma warning disable CS8618
    internal bool _isSubscribed;
    internal String _userName;
#pragma warning restore CS8618

    public bool IsSubscribed
    {
        set => this._isSubscribed = value;
    }

    public String UserName
    {
        set => this._userName = value;
    }
}
