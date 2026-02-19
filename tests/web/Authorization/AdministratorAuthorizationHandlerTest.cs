using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Web;

using Shipstone.OpenBook.Api.Test.Mocks;
using Shipstone.OpenBook.Api.WebTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.WebTest.Authorization;

public sealed class AdministratorAuthorizationHandlerTest
{
    private readonly IAuthorizationService _authorization;
    private readonly MockClaimsService _claims;
    private readonly MockLogger<DefaultAuthorizationService> _logger;

    public AdministratorAuthorizationHandlerTest()
    {
        IList<ServiceDescriptor> list = new List<ServiceDescriptor>();
        MockServiceCollection services = new();
        services._countFunc = () => list.Count;
        services._addAction = list.Add;
        services._itemFunc = i => list[i];
        services._getEnumeratorFunc = list.GetEnumerator;
        services.AddOpenBookWebAuthorization();
        MockClaimsService claims = new();
        services.AddSingleton<IClaimsService>(claims);
        MockLogger<DefaultAuthorizationService> logger = new();
        services.AddSingleton<ILogger<DefaultAuthorizationService>>(logger);
        IServiceProvider provider = new MockServiceProvider(services);

        this._authorization =
            provider.GetRequiredService<IAuthorizationService>();

        this._claims = claims;
        this._logger = logger;
    }

#region HandleRequirementAsync method
    [InlineData(Roles.Administrator, Roles.User)]
    [InlineData(Roles.SystemAdministrator, Roles.User)]
    [InlineData(Roles.SystemAdministrator, Roles.Administrator)]
    [Theory]
    public async Task TestHandleRequirementAsync_Authenticated_CreatorUser_Authorized(
        String role,
        String creatorRole
    )
    {
        // Arrange
        ClaimsPrincipal user = new();
        MockResource resource = new();
        this._claims._isAuthenticatedFunc = () => true;

        this._claims._rolesFunc = () =>
        {
            MockReadOnlySet<String> roles = new();
            roles._containsFunc = i => String.Equals(role, i);
            return roles;
        };

        resource._creatorRolesFunc = () =>
        {
            MockReadOnlySet<String> roles = new();
            roles._containsFunc = i => String.Equals(creatorRole, i);
            return roles;
        };

        this._logger._isEnabledFunc = _ => false;

        // Act
        AuthorizationResult result =
            await this._authorization.AuthorizeAsync(
                user,
                resource,
                Policies.Administrator
            );

        // Assert
        Assert.Null(result.Failure);
        Assert.True(result.Succeeded);
    }

    [InlineData(Roles.User, Roles.User)]
    [InlineData(Roles.User, Roles.Administrator)]
    [InlineData(Roles.User, Roles.SystemAdministrator)]
    [InlineData(Roles.Administrator, Roles.Administrator)]
    [InlineData(Roles.Administrator, Roles.SystemAdministrator)]
    [InlineData(Roles.SystemAdministrator, Roles.SystemAdministrator)]
    [Theory]
    public async Task TestHandleRequirementAsync_Authenticated_NotAuthorized(
        String role,
        String creatorRole
    )
    {
        // Arrange
        ClaimsPrincipal user = new();
        MockResource resource = new();
        this._claims._isAuthenticatedFunc = () => true;

        this._claims._rolesFunc = () =>
        {
            MockReadOnlySet<String> roles = new();
            roles._containsFunc = i => String.Equals(role, i);
            return roles;
        };

        resource._creatorRolesFunc = () =>
        {
            MockReadOnlySet<String> roles = new();
            roles._containsFunc = i => String.Equals(creatorRole, i);
            return roles;
        };

        this._logger._isEnabledFunc = _ => false;

        // Act
        AuthorizationResult result =
            await this._authorization.AuthorizeAsync(
                user,
                resource,
                Policies.Administrator
            );

        // Assert
        Assert.NotNull(result.Failure);
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task TestHandleRequirementAsync_NotAuthenticated()
    {
        // Arrange
        ClaimsPrincipal user = new();
        MockResource resource = new();
        this._claims._isAuthenticatedFunc = () => false;
        this._logger._isEnabledFunc = _ => false;

        // Act
        AuthorizationResult result =
            await this._authorization.AuthorizeAsync(
                user,
                resource,
                Policies.Administrator
            );

        // Assert
        Assert.NotNull(result.Failure);
        Assert.False(result.Succeeded);
    }
#endregion
}
