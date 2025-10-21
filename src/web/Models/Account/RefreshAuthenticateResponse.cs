using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Web.Models.Account;

internal sealed class RefreshAuthenticateResponse : AuthenticateResponseBase
{
    internal RefreshAuthenticateResponse(IAuthenticateResult result)
        : base(result)
    { }
}
