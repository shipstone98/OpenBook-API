using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Utilities;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

using Shipstone.OpenBook.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Users;

public sealed class UserRetrieveHandlerTest
{
    private readonly IUserRetrieveHandler _handler;
    private readonly MockRepository _repository;

    public UserRetrieveHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookCore();
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._handler = provider.GetRequiredService<IUserRetrieveHandler>();
        this._repository = repository;
    }

#region HandleAsync method
    [Fact]
    public async Task TestHandleAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._handler.HandleAsync(Guid.Empty, CancellationToken.None));

        // Assert
        Assert.Equal("identityId", ex.ParamName);
    }

#region Valid arguments
    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotActive()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieveForIdentityIdFunc = _ => new();
            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<UserNotActiveException>(() =>
            this._handler.HandleAsync(id, CancellationToken.None));
    }

    [Fact]
    public Task TestHandleAsync_Failure_UserNotFound()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieveForIdentityIdFunc = _ => null;
            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(id, CancellationToken.None));
    }

    [Fact]
    public async Task TestHandleAsync_Success_AssignedRoles()
    {
#region Arrange
        // Arrange
        Guid identityId = Guid.NewGuid();
        Guid id = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;
        DateTime created = now;
        DateTime updated = created.AddDays(12345);
        const String USER_NAME = "johndoe2025";
        DateTime consented = created.AddDays(1);

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieveForIdentityIdFunc = _ =>
                new UserEntity
                {
                    Consented = consented,
                    Created = created,
                    Id = id,
                    IsActive = true,
                    Updated = updated,
                    UserName = USER_NAME
                };

            return users;
        };

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
#endregion

        // Act
        IUser user =
            await this._handler.HandleAsync(
                identityId,
                CancellationToken.None
            );

        // Assert
        IEnumerable<String> roles = new String[]
        {
            Roles.Administrator,
            Roles.SystemAdministrator,
            Roles.User
        };

        user.AssertEqual(
            id,
            created,
            updated,
            USER_NAME,
            consented,
            roles
        );
    }

    [Fact]
    public async Task TestHandleAsync_Success_NotAssignedRoles()
    {
#region Arrange
        // Arrange
        Guid identityId = Guid.NewGuid();
        Guid id = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;
        DateTime created = now;
        DateTime updated = created.AddDays(12345);
        const String USER_NAME = "johndoe2025";
        DateTime consented = created.AddDays(1);

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieveForIdentityIdFunc = _ =>
                new UserEntity
                {
                    Consented = consented,
                    Created = created,
                    Id = id,
                    IsActive = true,
                    Updated = updated,
                    UserName = USER_NAME
                };

            return users;
        };

        this._repository._userRolesFunc = () =>
        {
            MockUserRoleRepository userRoles = new();
            userRoles._listForUserFunc = _ => Array.Empty<UserRoleEntity>();
            return userRoles;
        };
#endregion

        // Act
        IUser user =
            await this._handler.HandleAsync(
                identityId,
                CancellationToken.None
            );

        // Assert
        IEnumerable<String> roles = Array.Empty<String>();

        user.AssertEqual(
            id,
            created,
            updated,
            USER_NAME,
            consented,
            roles
        );
    }
#endregion
#endregion
}
