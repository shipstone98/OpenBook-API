using Xunit;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.WebTest;

internal static class Internals
{
    internal static void AssertNotAuthenticated(this IClaimsService claims)
    {
        Assert.Throws<UnauthorizedException>(() => claims.EmailAddress);
        Assert.Throws<UnauthorizedException>(() => claims.Id);
        Assert.False(claims.IsAuthenticated);
        Assert.Throws<UnauthorizedException>(() => claims.UserName);
    }
}
