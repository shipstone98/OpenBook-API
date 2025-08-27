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

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest;

public sealed class UserRepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly MockHMAC _hmac;
    private readonly IUserRepository _repository;

    public UserRepositoryTest()
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
        this._repository = provider.GetRequiredService<IUserRepository>();
    }

#region RetrieveAsync method
    [Fact]
    public async Task TestRetrieveAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.RetrieveAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("emailAddress", ex.ParamName);
    }

    [Fact]
    public async Task TestRetrieveAsync_Valid_Contains()
    {
        // Arrange
        const String EMAIL_ADDRESS = "john.doe@contoso.com";

        IEnumerable<UserEntity> users = new List<UserEntity>
        {
            new UserEntity
            {
                EmailAddress = EMAIL_ADDRESS
            }
        };

        this._hmac._hashCoreAction = (_, _, _) => { };
        this._hmac._hashFinalFunc = () => Array.Empty<byte>();
        this._hmac._initializeAction = () => { };

        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query = users.AsQueryable();
            MockDataSet<UserEntity> userDataSet = new(query);
            return userDataSet;
        };

        // Act
        UserEntity? user =
            await this._repository.RetrieveAsync(
                EMAIL_ADDRESS,
                CancellationToken.None
            );

        // Assert
        Assert.NotNull(user);
        Assert.Equal(EMAIL_ADDRESS, user.EmailAddress);
    }

    [Fact]
    public async Task TestRetrieveAsync_Valid_NotContains()
    {
        // Arrange
        this._hmac._hashCoreAction = (_, _, _) => { };
        this._hmac._hashFinalFunc = () => Array.Empty<byte>();
        this._hmac._initializeAction = () => { };

        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query =
                Array
                    .Empty<UserEntity>()
                    .AsQueryable();

            MockDataSet<UserEntity> userDataSet = new(query);
            return userDataSet;
        };

        // Act
        UserEntity? user =
            await this._repository.RetrieveAsync(
                "john.doe@contoso.com",
                CancellationToken.None
            );

        // Assert
        Assert.Null(user);
    }
#endregion

    [Fact]
    public async Task TestUpdateAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.UpdateAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("user", ex.ParamName);
    }

    [Fact]
    public async Task TestUpdateAsync_Valid()
    {
        // Arrange
        UserEntity user = new();

        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query =
                Array
                    .Empty<UserEntity>()
                    .AsQueryable();

            MockDataSet<UserEntity> userDataSet = new(query);
            userDataSet._setStateAction = (_, _) => { };
            return userDataSet;
        };

        // Act
        await this._repository.UpdateAsync(user, CancellationToken.None);

        // Nothing to assert
    }
}
