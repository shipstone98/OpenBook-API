using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    private readonly MockHMAC _hmac;
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
        MockHMAC hmac = new();
        services.AddSingleton<HMAC>(hmac);
        IServiceProvider provider = new MockServiceProvider(services);
        this._dataSource = dataSource;
        this._hmac = hmac;
        this._repository = provider.GetRequiredService<IUserRoleRepository>();
    }

    [Fact]
    public async Task TestCreateAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.CreateAsync(null!, CancellationToken.None));

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
        return this._repository.CreateAsync(user, CancellationToken.None);

        // Nothing to assert
    }
}
