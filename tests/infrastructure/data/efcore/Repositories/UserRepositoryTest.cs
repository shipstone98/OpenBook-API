using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

using Shipstone.Extensions.Security;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Repositories;

public sealed class UserRepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly INormalizationService _normalization;
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
        MockOptions<SecurityOptions> securityOptions = new();
        services.AddSingleton<IOptions<SecurityOptions>>(securityOptions);
        securityOptions._valueFunc = () => new();
        IServiceProvider provider = new MockServiceProvider(services);
        this._dataSource = dataSource;

        this._normalization =
            provider.GetRequiredService<INormalizationService>();

        this._repository = provider.GetRequiredService<IUserRepository>();
    }

    [Fact]
    public async Task TestCreateAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.CreateAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("user", ex.ParamName);
    }

    [Fact]
    public Task TestCreateAsync_Valid()
    {
        // Arrange
        UserEntity user = new();

        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query =
                Array
                    .Empty<UserEntity>()
                    .AsQueryable();

            MockDataSet<UserEntity> dataSet = new(query);
            dataSet._setStateAction = (_, _) => { };
            return dataSet;
        };

        // Act
        return this._repository.CreateAsync(user, CancellationToken.None);

        // Nothing to assert
    }

#region RetrieveAsync methods
#region Guid parameter
    [Fact]
    public async Task TestRetrieveAsync_Guid_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._repository.RetrieveAsync(
                    Guid.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("id", ex.ParamName);
    }

    [Fact]
    public async Task TestRetrieveAsync_Guid_Valid_Contains()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        this._dataSource._usersFunc = () =>
        {
            IEnumerable<UserEntity> users = new UserEntity[]
            {
                new UserEntity
                {
                    Id = id
                }
            };

            IQueryable<UserEntity> query = users.AsQueryable();
            return new MockDataSet<UserEntity>(query);
        };

        // Act
        UserEntity? user =
            await this._repository.RetrieveAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(id, user.Id);
    }

    [Fact]
    public async Task TestRetrieveAsync_Guid_Valid_NotContains()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query =
                Array
                    .Empty<UserEntity>()
                    .AsQueryable();

            return new MockDataSet<UserEntity>(query);
        };

        // Act
        UserEntity? user =
            await this._repository.RetrieveAsync(id, CancellationToken.None);

        // Assert
        Assert.Null(user);
    }
#endregion

#region String parameter
    [Fact]
    public async Task TestRetrieveAsync_String_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.RetrieveAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("emailAddress", ex.ParamName);
    }

    [Fact]
    public async Task TestRetrieveAsync_String_Valid_Contains()
    {
        // Arrange
        const String EMAIL_ADDRESS = "john.doe@contoso.com";

        this._dataSource._usersFunc = () =>
        {
            IEnumerable<UserEntity> users = new UserEntity[]
            {
                new UserEntity
                {
                    EmailAddress = EMAIL_ADDRESS,
                    EmailAddressNormalized =
                        this._normalization.Normalize(EMAIL_ADDRESS)
                }
            };

            IQueryable<UserEntity> query = users.AsQueryable();
            return new MockDataSet<UserEntity>(query);
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
    public async Task TestRetrieveAsync_String_Valid_NotContains()
    {
        // Arrange
        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query =
                Array
                    .Empty<UserEntity>()
                    .AsQueryable();

            return new MockDataSet<UserEntity>(query);
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
#endregion

#region RetrieveForName method
    [Fact]
    public async Task TestRetrieveForNameAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.RetrieveForNameAsync(
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("userName", ex.ParamName);
    }

    [Fact]
    public async Task TestRetrieveForNameAsync_Valid_Contains()
    {
        // Arrange
        const String USER_NAME = "johndoe2025";

        this._dataSource._usersFunc = () =>
        {
            IEnumerable<UserEntity> users = new UserEntity[]
            {
                new UserEntity
                {
                    UserName = USER_NAME,
                    UserNameNormalized =
                        this._normalization.Normalize(USER_NAME)
                }
            };

            IQueryable<UserEntity> query = users.AsQueryable();
            return new MockDataSet<UserEntity>(query);
        };

        // Act
        UserEntity? user =
            await this._repository.RetrieveForNameAsync(
                USER_NAME,
                CancellationToken.None
            );

        // Assert
        Assert.NotNull(user);
        Assert.Equal(USER_NAME, user.UserName);
    }

    [Fact]
    public async Task TestRetrieveForNameAsync_Valid_NotContains()
    {
        // Arrange
        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query =
                Array
                    .Empty<UserEntity>()
                    .AsQueryable();

            return new MockDataSet<UserEntity>(query);
        };

        // Act
        UserEntity? user =
            await this._repository.RetrieveForNameAsync(
                "johndoe2025",
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
    public Task TestUpdateAsync_Valid()
    {
        // Arrange
        UserEntity user = new();

        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query =
                Array
                    .Empty<UserEntity>()
                    .AsQueryable();

            MockDataSet<UserEntity> dataSet = new(query);
            dataSet._setStateAction = (_, _) => { };
            return dataSet;
        };

        // Act
        return this._repository.UpdateAsync(user, CancellationToken.None);

        // Nothing to assert
    }
}
