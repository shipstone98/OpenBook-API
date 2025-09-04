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

public sealed class ResourceOwnerAuthorizationHandlerTest
{
    private readonly IAuthorizationService _authorization;
    private readonly MockClaimsService _claims;
    private readonly MockLogger<DefaultAuthorizationService> _logger;

    public ResourceOwnerAuthorizationHandlerTest()
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
    [Fact]
    public async Task TestHandleRequirementAsync_Authenticated_Creator()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        ClaimsPrincipal user = new();
        MockResource resource = new();
        this._logger._isEnabledFunc = _ => false;
        this._claims._isAuthenticatedFunc = () => true;
        this._claims._idFunc = () => id;
        resource._creatorId = () => id;

        // Act
        AuthorizationResult result =
            await this._authorization.AuthorizeAsync(
                user,
                resource,
                Policies.ResourceOwner
            );

        // Assert
        Assert.Null(result.Failure);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task TestHandleRequirementAsync_Authenticated_NotCreator()
    {
        // Arrange
        ClaimsPrincipal user = new();
        MockResource resource = new();
        this._logger._isEnabledFunc = _ => false;
        this._claims._isAuthenticatedFunc = () => true;
        this._claims._idFunc = Guid.NewGuid;
        resource._creatorId = Guid.NewGuid;

        // Act
        AuthorizationResult result =
            await this._authorization.AuthorizeAsync(
                user,
                resource,
                Policies.ResourceOwner
            );

        // Assert
        Assert.NotNull(result.Failure);
        Assert.NotEmpty(result.Failure.FailedRequirements);
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task TestHandleRequirementAsync_NotAuthenticated()
    {
        // Arrange
        ClaimsPrincipal user = new();
        MockResource resource = new();
        this._logger._isEnabledFunc = _ => false;
        this._claims._isAuthenticatedFunc = () => false;

        // Act
        AuthorizationResult result =
            await this._authorization.AuthorizeAsync(
                user,
                resource,
                Policies.ResourceOwner
            );

        // Assert
        Assert.NotNull(result.Failure);
        Assert.NotEmpty(result.Failure.FailedRequirements);
        Assert.False(result.Succeeded);
    }
#endregion
}
