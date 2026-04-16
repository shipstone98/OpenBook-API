using Shipstone.Utilities;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Users;

namespace Shipstone.OpenBook.Api.Web.Services;

internal sealed class ClaimsService : IClaimsService
{
    internal IUser? _user;

    bool IClaimsService.IsAuthenticated => this._user is not null;

    IUser IClaimsService.User
    {
        get
        {
            if (this._user is null)
            {
                throw new UnauthorizedException("The current user is not authenticated.");
            }

            return this._user;
        }
    }
}
