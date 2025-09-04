using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

using Shipstone.Extensions.Pagination;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Repositories;

public sealed class RepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly IRepository _repository;

    public RepositoryTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookInfrastructureDataEntityFrameworkCore();
        MockDataSource dataSource = new();
        services.AddSingleton<IDataSource>(dataSource);
        MockOptionsSnapshot<PaginationOptions> options = new();
        services.AddSingleton<IOptionsSnapshot<PaginationOptions>>(options);
        PaginationOptions optionsValue = new();
        options._valueFunc = () => optionsValue;
        services.AddSingleton<HMAC, MockHMAC>();
        IServiceProvider provider = new MockServiceProvider(services);
        this._dataSource = dataSource;
        this._repository = provider.GetRequiredService<IRepository>();
    }

    [Fact]
    public void TestPosts_Get()
    {
        // Act and assert
        Assert.NotNull(this._repository.Posts);
    }

    [Fact]
    public void TestRoles_Get()
    {
        // Act and assert
        Assert.NotNull(this._repository.Roles);
    }

    [Fact]
    public void TestUserFollowings_Get()
    {
        // Act and assert
        Assert.NotNull(this._repository.UserFollowings);
    }

    [Fact]
    public void TestUserRefreshTokens_Get()
    {
        // Act and assert
        Assert.NotNull(this._repository.UserRefreshTokens);
    }

    [Fact]
    public void TestUserRoles_Get()
    {
        // Act and assert
        Assert.NotNull(this._repository.UserRoles);
    }

    [Fact]
    public void TestUsers_Get()
    {
        // Act and assert
        Assert.NotNull(this._repository.Users);
    }

    [Fact]
    public Task TestSaveAsync()
    {
        // Arrange
        this._dataSource._saveAction = () => { };

        // Act
        return this._repository.SaveAsync(CancellationToken.None);

        // Nothing to assert
    }
}
