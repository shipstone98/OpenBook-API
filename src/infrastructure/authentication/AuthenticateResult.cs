using System;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Infrastructure.Authentication;

internal sealed record AuthenticateResult(
    String AccessToken,
    String RefreshToken,
    DateTime RefreshTokenExpires
)
    : IAuthenticateResult;
