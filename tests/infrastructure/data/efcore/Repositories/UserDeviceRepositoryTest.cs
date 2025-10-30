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

public sealed class UserDeviceRepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly IUserDeviceRepository _repository;

    public UserDeviceRepositoryTest()
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
        this._repository = provider.GetRequiredService<IUserDeviceRepository>();
    }

    [Fact]
    public async Task TestDeleteAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.DeleteAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("userDevice", ex.ParamName);
    }

    [Fact]
    public Task TestDeleteAsync_Valid()
    {
        // Arrange
        UserDeviceEntity user = new();

        this._dataSource._userDevicesFunc = () =>
        {
            IQueryable<UserDeviceEntity> query =
                Array
                    .Empty<UserDeviceEntity>()
                    .AsQueryable();

            MockDataSet<UserDeviceEntity> dataSet = new(query);
            dataSet._setStateAction = (_, _) => { };
            return dataSet;
        };

        // Act
        return this._repository.DeleteAsync(user, CancellationToken.None);

        // Nothing to assert
    }
}
