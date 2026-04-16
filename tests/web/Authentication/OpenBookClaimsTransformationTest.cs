using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Web;

using Shipstone.OpenBook.Api.Test.Mocks;
using Shipstone.OpenBook.Api.WebTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.WebTest.Authentication;

public sealed class OpenBookClaimsTransformationTest
{
    private readonly MockUserRetrieveHandler _handler;
    private readonly IClaimsTransformation _transformation;

    public OpenBookClaimsTransformationTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookWebClaims();
        MockUserRetrieveHandler handler = new();
        services.AddSingleton<IUserRetrieveHandler>(handler);
        IServiceProvider provider = new MockServiceProvider(services);
        this._handler = handler;

        this._transformation =
            provider.GetRequiredService<IClaimsTransformation>();
    }

#region TransformAsync method
    [Fact]
    public async Task TestTransformAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._transformation.TransformAsync(null!));

        // Assert
        Assert.Equal("principal", ex.ParamName);
    }

#region Valid arguments
    [Fact]
    public async Task TestTransformAsync_Valid_IdentityIdNotValid()
    {
        // Arrange
        ClaimsPrincipal principal = new();

        // Act
        ClaimsPrincipal result =
            await this._transformation.TransformAsync(principal);

        // Assert
        Assert.Empty(result.Claims);
    }

    [Fact]
    public async Task TestTransformAsync_Valid_IdentityIdValid_ExceptionThrown()
    {
        // Arrange
        String identityIdString =
            Guid
                .NewGuid()
                .ToString();

        Claim claim = new(ClaimTypes.NameIdentifier, identityIdString);
        IEnumerable<Claim> principalClaims = new Claim[1] { claim };
        IIdentity identity = new ClaimsIdentity(principalClaims);
        ClaimsPrincipal principal = new(identity);

        // Act
        ClaimsPrincipal result =
            await this._transformation.TransformAsync(principal);

        // Assert
        Assert.Single(
            result.Claims,
            c =>
                c.Type.Equals(ClaimTypes.NameIdentifier)
                && c.Value.Equals(identityIdString)
        );
    }

    [Fact]
    public async Task TestTransformAsync_Valid_IdentityIdValid_ExceptionNotThrown()
    {
#region Arrange
        // Arrange
        String identityIdString =
            Guid
                .NewGuid()
                .ToString();

        Claim nameIdClaim = new(ClaimTypes.NameIdentifier, identityIdString);
        Claim roleClaim = new(ClaimTypes.Role, Roles.User);

        IEnumerable<Claim> principalClaims =
            new Claim[2] { nameIdClaim, roleClaim };

        IIdentity identity = new ClaimsIdentity(principalClaims);
        ClaimsPrincipal principal = new(identity);

        this._handler._handleFunc = _ =>
        {
            MockUser user = new();

            user._rolesFunc = () =>
            {
                MockReadOnlySet<String> roles = new();

                roles._getEnumeratorFunc = () =>
                {
                    IEnumerable<String> collection = new String[]
                    {
                        Roles.Administrator,
                        Roles.SystemAdministrator,
                        Roles.User
                    };

                    return collection.GetEnumerator();
                };

                return roles;
            };

            return user;
        };
#endregion

        // Act
        ClaimsPrincipal result =
            await this._transformation.TransformAsync(principal);

        // Assert
        IEnumerable<String> roles =
            result.Claims
                .Where(c => c.Type.Equals(ClaimTypes.Role))
                .Select(c => c.Value);

        Assert.Single(roles, Roles.Administrator.Equals);
        Assert.Single(roles, Roles.SystemAdministrator.Equals);
        Assert.Single(roles, Roles.User.Equals);

        Assert.Single(
            result.Claims,
            c =>
                c.Type.Equals(ClaimTypes.NameIdentifier)
                && c.Value.Equals(identityIdString)
        );
    }
#endregion
#endregion
}
