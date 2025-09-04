using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Authorization;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

using Shipstone.OpenBook.Api.Infrastructure.AuthorizationTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.AuthorizationTest;

public sealed class AuthorizationServiceTest
{
    private readonly IAuthorizationService _authorization;
    private readonly MockAuthorizationService _mockAuthorization;

    public AuthorizationServiceTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookInfrastructureAuthorization();
        MockAuthorizationService mockAuthorization = new();
        services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationService>(mockAuthorization);
        IServiceProvider provider = new MockServiceProvider(services);

        this._authorization =
            provider.GetRequiredService<IAuthorizationService>();

        this._mockAuthorization = mockAuthorization;
    }

#region AuthorizeAsync method
    [Fact]
    public async Task TestAuthorizeAsync_Invalid_EntityNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._authorization.AuthorizeAsync<int>(
                    null!,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("entity", ex.ParamName);
    }

    [Fact]
    public async Task TestAuthorizeAsync_Invalid_PolicyNull()
    {
        // Arrange
        CreatableEntity<int> entity = new MockCreatableEntity<int>();

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._authorization.AuthorizeAsync(
                    entity,
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("policy", ex.ParamName);
    }

    [Fact]
    public Task TestAuthorizeAsync_Valid_Failure()
    {
        // Arrange
        CreatableEntity<int> entity = new MockCreatableEntity<int>();

        this._mockAuthorization._authorizeFunc = (_, _, _) =>
            Microsoft.AspNetCore.Authorization.AuthorizationResult.Failed();

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._authorization.AuthorizeAsync(
                entity,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestAuthorizeAsync_Valid_Success()
    {
        // Arrange
        CreatableEntity<int> entity = new MockCreatableEntity<int>();

        this._mockAuthorization._authorizeFunc = (_, _, _) =>
            Microsoft.AspNetCore.Authorization.AuthorizationResult.Success();

        // Act
        return this._authorization.AuthorizeAsync(
            entity,
            String.Empty,
            CancellationToken.None
        );

        // Nothing to assert
    }
#endregion
}
