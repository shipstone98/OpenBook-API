using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

using Shipstone.OpenBook.Api.CoreTest.Mocks;
using Shipstone.OpenBook.Api.Test.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Users;

public sealed class UserUpdateHandlerTest
{
    private readonly MockClaimsService _claims;
    private readonly IUserUpdateHandler _handler;
    private readonly MockRepository _repository;

    public UserUpdateHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookCore();
        MockClaimsService claims = new();
        services.AddSingleton<IClaimsService>(claims);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._claims = claims;
        this._handler = provider.GetRequiredService<IUserUpdateHandler>();
        this._repository = repository;
    }

#region HandleAsync method
#region Invalid arguments
    [Fact]
    public Task TestHandleAsync_Invalid_ForenameInvalid() => throw new NotImplementedException();

    [Fact]
    public async Task TestHandleAsync_Invalid_ForenameNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    null!,
                    "Doe",
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("forename", ex.ParamName);
    }

    [Fact]
    public Task TestHandleAsync_Invalid_SurnameInvalid() => throw new NotImplementedException();

    [Fact]
    public async Task TestHandleAsync_Invalid_SurnameNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    "John",
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("surname", ex.ParamName);
    }
#endregion

#region Valid arguments
#region Failure
    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotActive()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => new();
            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        // Act and assert
        return Assert.ThrowsAsync<UserNotActiveException>(() =>
            this._handler.HandleAsync("John", "Doe", CancellationToken.None));
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Failure_UserNotAuthenticated()
    {
        // Arrange
        Exception innerException = new UnauthorizedException();
        this._repository._usersFunc = () => new MockUserRepository();
        this._claims._idFunc = () => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                this._handler.HandleAsync(
                    "John",
                    "Doe",
                    CancellationToken.None
                ));

        // Assert
        Assert.Same(innerException, ex);
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotFound()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => null;
            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync("John", "Doe", CancellationToken.None));
    }
#endregion

    [Fact]
    public async Task TestHandleAsync_Valid_Success()
    {
#region Arrange
        // Arrange
        Guid id = Guid.NewGuid();
        DateTime created = DateTime.UnixEpoch.ToUniversalTime();
        const String EMAIL_ADDRESS = "john.doe@contoso.com";
        const String USER_NAME = "johndoe2025";
        const String FORENAME = " John ";
        const String SURNAME = " Doe ";
        DateTime consented = created.AddDays(1);

        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow)
                .AddYears(-18);

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = id =>
                new UserEntity
                {
                    Born = born,
                    Consented = consented,
                    Created = created,
                    EmailAddress = EMAIL_ADDRESS,
                    Id = id,
                    IsActive = true,
                    UserName = USER_NAME
                };

            users._updateAction = _ => { };
            return users;
        };

        this._claims._idFunc = () => id;
        this._repository._saveAction = () => { };

        this._repository._userRolesFunc = () =>
        {
            MockUserRoleRepository userRoles = new();

            userRoles._listForUserFunc = _ =>
                new UserRoleEntity[]
                {
                    new UserRoleEntity
                    {
                        RoleId = Roles.AdministratorId
                    },
                    new UserRoleEntity
                    {
                        RoleId = Roles.SystemAdministratorId
                    },
                    new UserRoleEntity
                    {
                        RoleId = Roles.UserId
                    },
                };

            return userRoles;
        };

        this._repository._rolesFunc = () =>
        {
            MockRoleRepository roles = new();

            roles._retrieveFunc = id =>
            {
                switch (id)
                {
                    case Roles.AdministratorId:
                        return new RoleEntity
                        {
                            Name = Roles.Administrator
                        };

                    case Roles.SystemAdministratorId:
                        return new RoleEntity
                        {
                            Name = Roles.SystemAdministrator
                        };

                    case Roles.UserId:
                        return new RoleEntity
                        {
                            Name = Roles.User
                        };

                    default:
                        return null;
                }
            };

            return roles;
        };

        DateTime notBefore = DateTime.UtcNow;
#endregion

        // Act
        IUser user =
            await this._handler.HandleAsync(
                FORENAME,
                SURNAME,
                CancellationToken.None
            );

        // Assert
        Assert.False(DateTime.Compare(notBefore, user.Updated) > 0);

        IEnumerable<String> roles = new String[]
        {
            Roles.Administrator,
            Roles.SystemAdministrator,
            Roles.User
        };

        user.AssertEqual(
            id,
            created,
            user.Updated,
            EMAIL_ADDRESS,
            USER_NAME,
            FORENAME.Trim(),
            SURNAME.Trim(),
            born,
            consented,
            roles
        );
    }
#endregion
#endregion
}
