using Xunit;

using Shipstone.Utilities;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.WebTest;

internal static class Internals
{
    internal static void AssertNotAuthenticated(this IClaimsService claims)
    {
        Assert.False(claims.IsAuthenticated);
        Assert.Throws<UnauthorizedException>(() => claims.User);
    }
}
