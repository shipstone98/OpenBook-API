using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest;

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
        services.AddSingleton<HMAC, MockHMAC>();
        IServiceProvider provider = new MockServiceProvider(services);
        this._dataSource = dataSource;
        this._repository = provider.GetRequiredService<IRepository>();
    }

    [Fact]
    public void TestUsers_Get()
    {
        // Act
        IUserRepository users = this._repository.Users;

        // Assert
        Assert.NotNull(users);
    }

    [Fact]
    public async Task TestSaveAsync()
    {
        // Arrange
        this._dataSource._saveAction = () => { };

        // Act
        await this._repository.SaveAsync(CancellationToken.None);

        // Nothing to assert
    }
}
