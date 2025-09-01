using System;
using System.Collections.Generic;
using System.Linq;
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
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Users;

public sealed class UserRetrieveHandlerTest
{
    private readonly MockClaimsService _claims;
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
        MockClaimsService claims = new();
        services.AddSingleton<IClaimsService>(claims);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._claims = claims;
        this._handler = provider.GetRequiredService<IUserRetrieveHandler>();
        this._repository = repository;
    }

#region HandleAsync method
#region Failure
    [Fact]
    public async Task TestHandleAsync_Failure_UserNotActive()
    {
        // Arrange
        this._claims._idFunc = Guid.NewGuid;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => new();
            return users;
        };

        // Act and assert
        await Assert.ThrowsAsync<UserNotActiveException>(() =>
            this._handler.HandleAsync(CancellationToken.None));
    }

    [Fact]
    public async Task TestHandleAsync_Failure_UserNotAuthenticated()
    {
        // Arrange
        Exception innerException = new UnauthorizedException();
        this._claims._idFunc = () => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                this._handler.HandleAsync(CancellationToken.None));

        // Assert
        Assert.True(Object.ReferenceEquals(innerException, ex));
    }

    [Fact]
    public async Task TestHandleAsync_Failure_UserNotFound()
    {
        // Arrange
        this._claims._idFunc = Guid.NewGuid;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => null;
            return users;
        };

        // Act and assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(CancellationToken.None));
    }
#endregion

    [Fact]
    public async Task TestHandleAsync_Success_AssignedRoles()
    {
#region Arrange
        // Arrange
        Guid id = Guid.NewGuid();
        DateTime created = DateTime.UtcNow;
        DateTime updated = created.AddDays(12345);
        const String EMAIL_ADDRESS = "john.doe@contoso.com";
        const String USER_NAME = "johndoe2025";
        const String FORENAME = "John";
        const String SURNAME = "Doe";
        DateTime consented = created.AddDays(1);
        this._claims._idFunc = () => id;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = id =>
                new UserEntity
                {
                    Consented = consented,
                    Created = created,
                    EmailAddress = EMAIL_ADDRESS,
                    Forename = FORENAME,
                    Id = id,
                    IsActive = true,
                    Surname = SURNAME,
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
        IUser user = await this._handler.HandleAsync(CancellationToken.None);

        // Assert
        IEnumerable<String> roles = new String[]
        {
            Roles.Administrator,
            Roles.SystemAdministrator,
            Roles.User
        };

        Assert.Equal(consented, user.Consented);
        Assert.Equal(DateTimeKind.Utc, user.Consented.Kind);
        Assert.Equal(created, user.Created);
        Assert.Equal(DateTimeKind.Utc, user.Created.Kind);
        Assert.Equal(EMAIL_ADDRESS, user.EmailAddress);
        Assert.Equal(FORENAME, user.Forename);
        Assert.Equal(id, user.Id);
        Assert.True(roles.SequenceEqual(user.Roles));
        Assert.Equal(SURNAME, user.Surname);
        Assert.Equal(updated, user.Updated);
        Assert.Equal(USER_NAME, user.UserName);
    }

    [Fact]
    public async Task TestHandleAsync_Success_NotAssignedRoles()
    {
#region Arrange
        // Arrange
        Guid id = Guid.NewGuid();
        DateTime created = DateTime.UtcNow;
        DateTime updated = created.AddDays(12345);
        const String EMAIL_ADDRESS = "john.doe@contoso.com";
        const String USER_NAME = "johndoe2025";
        const String FORENAME = "John";
        const String SURNAME = "Doe";
        DateTime consented = created.AddDays(1);
        this._claims._idFunc = () => id;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = id =>
                new UserEntity
                {
                    Consented = consented,
                    Created = created,
                    EmailAddress = EMAIL_ADDRESS,
                    Forename = FORENAME,
                    Id = id,
                    IsActive = true,
                    Surname = SURNAME,
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
        IUser user = await this._handler.HandleAsync(CancellationToken.None);

        // Assert
        Assert.Equal(consented, user.Consented);
        Assert.Equal(DateTimeKind.Utc, user.Consented.Kind);
        Assert.Equal(created, user.Created);
        Assert.Equal(DateTimeKind.Utc, user.Created.Kind);
        Assert.Equal(EMAIL_ADDRESS, user.EmailAddress);
        Assert.Equal(FORENAME, user.Forename);
        Assert.Equal(id, user.Id);
        Assert.Empty(user.Roles);
        Assert.Equal(SURNAME, user.Surname);
        Assert.Equal(updated, user.Updated);
        Assert.Equal(USER_NAME, user.UserName);
    }
#endregion
}
