using System;

namespace Shipstone.OpenBook.Api.Web.Models.Account;

internal sealed class OtpGenerateRequest
{
#pragma warning disable CS8618
    internal String _emailAddress;
#pragma warning restore CS8618

    public String EmailAddress
    {
        set => this._emailAddress = value;
    }
}
