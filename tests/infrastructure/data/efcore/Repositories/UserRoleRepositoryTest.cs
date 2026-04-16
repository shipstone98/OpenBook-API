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

public sealed class UserRoleRepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly IUserRoleRepository _repository;

    public UserRoleRepositoryTest()
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
        this._repository = provider.GetRequiredService<IUserRoleRepository>();
    }

    [Fact]
    public async Task TestCreateAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.CreateAsync(
                    null!,
                    TestContext.Current.CancellationToken
                ));

        // Assert
        Assert.Equal("userRole", ex.ParamName);
    }

    [Fact]
    public Task TestCreateAsync_Valid()
    {
        // Arrange
        UserRoleEntity user = new();

        this._dataSource._userRolesFunc = () =>
        {
            IQueryable<UserRoleEntity> query =
                Array
                    .Empty<UserRoleEntity>()
                    .AsQueryable();

            MockDataSet<UserRoleEntity> dataSet = new(query);
            dataSet._setStateAction = (_, _) => { };
            return dataSet;
        };

        // Act
        return this._repository.CreateAsync(
            user,
            TestContext.Current.CancellationToken
        );

        // Nothing to assert
    }

    [Fact]
    public async Task TestDeleteAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.DeleteAsync(
                    null!,
                    TestContext.Current.CancellationToken
                ));

        // Assert
        Assert.Equal("userRole", ex.ParamName);
    }

    [Fact]
    public Task TestDeleteAsync_Valid()
    {
        // Arrange
        UserRoleEntity user = new();

        this._dataSource._userRolesFunc = () =>
        {
            IQueryable<UserRoleEntity> query =
                Array
                    .Empty<UserRoleEntity>()
                    .AsQueryable();

            MockDataSet<UserRoleEntity> dataSet = new(query);
            dataSet._setStateAction = (_, _) => { };
            return dataSet;
        };

        // Act
        return this._repository.DeleteAsync(
            user,
            TestContext.Current.CancellationToken
        );

        // Nothing to assert
    }
}
