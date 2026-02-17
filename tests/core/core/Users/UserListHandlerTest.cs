using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Utilities.Collections;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

using Shipstone.OpenBook.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Users;

public sealed class UserListHandlerTest
{
    private readonly IUserListHandler _handler;
    private readonly MockRepository _repository;

    public UserListHandlerTest()
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
        this._handler = provider.GetRequiredService<IUserListHandler>();
        this._repository = repository;
    }

    [Fact]
    public async Task TestHandleAsync_Empty()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._listFunc = () =>
            {
                MockReadOnlyPaginatedList<UserEntity> users = new();

                users._getEnumeratorFunc = () =>
                {
                    IEnumerable<UserEntity> collection =
                        Array.Empty<UserEntity>();

                    return collection.GetEnumerator();
                };

                users._totalCountFunc = () => 0;
                users._pageIndexFunc = () => 0;
                users._pageCountFunc = () => 1;
                return users;
            };

            return users;
        };

        // Act
        IReadOnlyPaginatedList<IUser> users =
            await this._handler.HandleAsync(CancellationToken.None);

        // Assert
        users.AssertEmpty();
    }

    [Fact]
    public async Task TestHandleAsync_NotEmpty()
    {
#region Arrange
        // Arrange
        const int TOTAL_COUNT = 5;
        const int PAGE_INDEX = 1;
        const int PAGE_COUNT = 2;
        Guid systemAdministratorId = Guid.NewGuid();

        DateTime systemAdministratorCreated =
            DateTime.UnixEpoch.ToUniversalTime();

        DateTime systemAdministratorUpdated =
            systemAdministratorCreated.AddDays(10);

        const String SYSTEM_ADMINISTRATOR_EMAIL_ADDRESS =
            "jane.doe@contoso.com";

        const String SYSTEM_ADMINISTRATOR_USER_NAME = "janedoe2025";
        const String SYSTEM_ADMINISTRATOR_FORENAME = "Jane";
        const String SYSTEM_ADMINISTRATOR_SURNAME = "Doe";
        DateOnly systemAdministratorBorn = new(1960, 1, 2);

        DateTime systemAdministratorConsented =
            systemAdministratorCreated.AddDays(1);

        Guid userId = Guid.NewGuid();
        DateTime userCreated = DateTime.UtcNow;
        DateTime userUpdated = userCreated.AddDays(10);
        const String USER_EMAIL_ADDRESS = "john.doe@contoso.com";
        const String USER_USER_NAME = "johndoe2025";
        const String USER_FORENAME = "John";
        const String USER_SURNAME = "Doe";
        DateOnly userBorn = new(1970, 3, 4);
        DateTime userConsented = userCreated.AddDays(1);

        IEnumerable<UserEntity> userCollection = new UserEntity[]
        {
            new UserEntity
            {
                Born = systemAdministratorBorn,
                Consented = systemAdministratorConsented,
                Created = systemAdministratorCreated,
                EmailAddress = SYSTEM_ADMINISTRATOR_EMAIL_ADDRESS,
                Forename = SYSTEM_ADMINISTRATOR_FORENAME,
                Id = systemAdministratorId,
                Surname = SYSTEM_ADMINISTRATOR_SURNAME,
                Updated = systemAdministratorUpdated,
                UserName = SYSTEM_ADMINISTRATOR_USER_NAME
            },
            new UserEntity
            {
                Born = userBorn,
                Consented = userConsented,
                Created = userCreated,
                EmailAddress = USER_EMAIL_ADDRESS,
                Forename = USER_FORENAME,
                Id = userId,
                Surname = USER_SURNAME,
                Updated = userUpdated,
                UserName = USER_USER_NAME
            }
        };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._listFunc = () =>
            {
                MockReadOnlyPaginatedList<UserEntity> users = new();
                users._getEnumeratorFunc = userCollection.GetEnumerator;
                users._totalCountFunc = () => TOTAL_COUNT;
                users._pageIndexFunc = () => PAGE_INDEX;
                users._pageCountFunc = () => PAGE_COUNT;
                return users;
            };

            return users;
        };

        this._repository._userRolesFunc = () =>
        {
            MockUserRoleRepository userRoles = new();

            userRoles._listForUserFunc = id =>
            {
                if (Guid.Equals(id, systemAdministratorId))
                {
                    return new UserRoleEntity[]
                    {
                        new UserRoleEntity
                        {
                            RoleId = Roles.SystemAdministratorId
                        },
                        new UserRoleEntity
                        {
                            RoleId = Roles.AdministratorId
                        },
                        new UserRoleEntity
                        {
                            RoleId = Roles.UserId
                        }
                    };
                }

                if (Guid.Equals(id, userId))
                {
                    return new UserRoleEntity[]
                    {
                        new UserRoleEntity
                        {
                            RoleId = Roles.UserId
                        }
                    };
                }

                return Array.Empty<UserRoleEntity>();
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
        IReadOnlyPaginatedList<IUser> users =
            await this._handler.HandleAsync(CancellationToken.None);

#region Assert
        // Assert
        users.AssertEqual(2, TOTAL_COUNT, PAGE_INDEX, PAGE_COUNT);

        IEnumerable<String> systemAdministratorRoles =
            new String[]
                { Roles.Administrator, Roles.SystemAdministrator, Roles.User };

        IEnumerable<String> userRoles = new String[] { Roles.User };

        users[0].AssertEqual(
            systemAdministratorId,
            systemAdministratorCreated,
            systemAdministratorUpdated,
            SYSTEM_ADMINISTRATOR_EMAIL_ADDRESS,
            SYSTEM_ADMINISTRATOR_USER_NAME,
            SYSTEM_ADMINISTRATOR_FORENAME,
            SYSTEM_ADMINISTRATOR_SURNAME,
            systemAdministratorBorn,
            systemAdministratorConsented,
            systemAdministratorRoles
        );

        users[1].AssertEqual(
            userId,
            userCreated,
            userUpdated,
            USER_EMAIL_ADDRESS,
            USER_USER_NAME,
            USER_FORENAME,
            USER_SURNAME,
            userBorn,
            userConsented,
            userRoles
        );
#endregion
    }
}
