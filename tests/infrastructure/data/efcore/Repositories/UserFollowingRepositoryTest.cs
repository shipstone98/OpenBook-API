using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Repositories;

public sealed class UserFollowingRepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly IUserFollowingRepository _repository;

    public UserFollowingRepositoryTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookInfrastructureDataEntityFrameworkCore();
        MockDataSource dataSource = new();
        services.AddSingleton<IDataSource>(dataSource);
        IServiceProvider provider = new MockServiceProvider(services);
        this._dataSource = dataSource;

        this._repository =
            provider.GetRequiredService<IUserFollowingRepository>();
    }

    [Fact]
    public async Task TestCreateAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.CreateAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("userFollowing", ex.ParamName);
    }

    [Fact]
    public Task TestCreateAsync_Valid()
    {
        // Arrange
        UserFollowingEntity userFollowing = new();

        this._dataSource._userFollowingsFunc = () =>
        {
            IQueryable<UserFollowingEntity> query =
                Array
                    .Empty<UserFollowingEntity>()
                    .AsQueryable();

            MockDataSet<UserFollowingEntity> dataSet = new(query);
            dataSet._setStateAction = (_, _) => { };
            return dataSet;
        };

        // Act
        return this._repository.CreateAsync(
            userFollowing,
            CancellationToken.None
        );

        // Nothing to assert
    }
}
