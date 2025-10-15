using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Extensions.Identity;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Passwords;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

using Shipstone.OpenBook.Api.CoreTest.Mocks;
using Shipstone.OpenBook.Api.Test.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Passwords;

public sealed class PasswordUpdateHandlerTest
{
    private readonly MockClaimsService _claims;
    private readonly IPasswordUpdateHandler _handler;
    private readonly MockPasswordService _password;
    private readonly MockRepository _repository;

    public PasswordUpdateHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookCore();
        MockClaimsService claims = new();
        services.AddSingleton<IClaimsService>(claims);
        MockPasswordService password = new();
        services.AddSingleton<IPasswordService>(password);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._claims = claims;
        this._handler = provider.GetRequiredService<IPasswordUpdateHandler>();
        this._password = password;
        this._repository = repository;
    }

#region HandleAsync method
    [Fact]
    public async Task TestHandleAsync_Invalid_PasswordCurrentNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    null!,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("passwordCurrent", ex.ParamName);
    }

    [Fact]
    public async Task TestHandleAsync_Invalid_PasswordNewInvalid()
    {
        // Arrange
        ArgumentException innerException = new();
        this._password._validateAction = _ => throw innerException;

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.True(Object.ReferenceEquals(innerException, ex));
    }

#region Valid arguments
#region Failure
    [Fact]
    public async Task TestHandleAsync_Valid_Failure_PasswordNotCorrect()
    {
        // Arrange
        IncorrectPasswordException innerException = new();
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    PasswordHash = String.Empty
                };

            return users;
        };

        this._claims._idFunc = Guid.NewGuid;
        this._password._verifyFunc = (_, _) => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<IncorrectPasswordException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.True(Object.ReferenceEquals(innerException, ex.InnerException));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotActive()
    {
        // Arrange
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => new();
            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        // Act and assert
        return Assert.ThrowsAsync<UserNotActiveException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Failure_UserNotAuthenticated()
    {
        // Arrange
        UnauthorizedException innerException = new();
        this._password._validateAction = _ => { };
        this._repository._usersFunc = () => new MockUserRepository();
        this._claims._idFunc = () => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.True(Object.ReferenceEquals(innerException, ex));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotFound()
    {
        // Arrange
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => null;
            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotVerified()
    {
        // Arrange
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = _ =>
                new UserEntity
                {
                    IsActive = true
                };

            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }
#endregion

    [Fact]
    public async Task TestHandleAsync_Valid_Success()
    {
#region Arrange
        // Arrange
        Guid id = Guid.NewGuid();
        DateTime created = DateTime.UnixEpoch.ToUniversalTime();
        const String EMAIL_ADDRESS = "john.doe@lampada.co";
        const String USER_NAME = "johndoe2025";
        const String FORENAME = "John";
        const String SURNAME = "Doe";
        DateTime consented = created.AddDays(1);

        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow)
                .AddYears(-18);

        this._password._validateAction = _ => { };

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
                    Forename = FORENAME,
                    Id = id,
                    IsActive = true,
                    PasswordHash = String.Empty,
                    Surname = SURNAME,
                    Updated = created,
                    UserName = USER_NAME
                };

            users._updateAction = _ => { };
            return users;
        };

        this._claims._idFunc = () => id;
        this._password._verifyFunc = (_, _) => false;
        this._password._hashFunc = _ => String.Empty;
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
                    }
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
                String.Empty,
                String.Empty,
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
            FORENAME,
            SURNAME,
            born,
            consented,
            roles
        );
    }
#endregion
#endregion
}
