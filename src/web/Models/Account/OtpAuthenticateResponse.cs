using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Web.Models.Account;

internal sealed class OtpAuthenticateResponse : AuthenticateResponseBase
{
    internal OtpAuthenticateResponse(IAuthenticateResult result) : base(result)
    { }
}
