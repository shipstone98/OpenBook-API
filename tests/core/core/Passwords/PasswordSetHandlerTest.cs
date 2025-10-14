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
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Passwords;

public sealed class PasswordSetHandlerTest
{
    private readonly IPasswordSetHandler _handler;
    private readonly MockPasswordService _password;
    private readonly MockRepository _repository;

    public PasswordSetHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookCore();
        MockPasswordService password = new();
        services.AddSingleton<IPasswordService>(password);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._handler = provider.GetRequiredService<IPasswordSetHandler>();
        this._password = password;
        this._repository = repository;
    }

#region HandleAsync method
#region Invalid arguments
    [Fact]
    public async Task TestHandleAsync_Invalid_EmailAddressNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    null!,
                    String.Empty,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("emailAddress", ex.ParamName);
    }

    [Fact]
    public async Task TestHandleAsync_Invalid_OtpNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    null!,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("otp", ex.ParamName);
    }

    [Fact]
    public async Task TestHandleAsync_Invalid_PasswordInvalid()
    {
        // Arrange
        ArgumentException innerException = new(null as String, "password");
        this._password._validateAction = _ => throw innerException;

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    String.Empty,
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.True(Object.ReferenceEquals(innerException, ex));
        Assert.Equal("password", ex.ParamName);
    }
#endregion

#region Valid arguments
#region Failure
    [Fact]
    public Task TestHandleAsync_Valid_Failure_EmailAddressNotFound()
    {
        // Arrange
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_StringFunc = _ => null;
            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_OtpExpired_OtpExpiresNotNull()
    {
        // Arrange
        const String OTP = "123456";
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    Otp = OTP,
                    OtpExpires = DateTime.MinValue
                };

            users._updateAction = _ => { };
            return users;
        };

        this._repository._saveAction = () => { };

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                OTP,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_OtpExpired_OtpExpiresNull()
    {
        // Arrange
        const String OTP = "123456";
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    Otp = OTP
                };

            users._updateAction = _ => { };
            return users;
        };

        this._repository._saveAction = () => { };

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                OTP,
                String.Empty,
                CancellationToken.None
            ));
    }

    [InlineData(null)]
    [InlineData("123456")]
    [Theory]
    public Task TestHandleAsync_Valid_Failure_OtpNotEqual(String? otp)
    {
        // Arrange
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    Otp = otp
                };

            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotActive()
    {
        // Arrange
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_StringFunc = _ => new();
            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<UserNotActiveException>(() =>
            this._handler.HandleAsync(
                String.Empty,
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
        const String EMAIL_ADDRESS = "john.doe@contoso.com";
        const String USER_NAME = "johndoe2025";
        const String FORENAME = "John";
        const String SURNAME = "Doe";
        const String OTP = "123456";
        DateTime consented = created.AddDays(1);

        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow)
                .AddYears(-18);

        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = ea =>
                new UserEntity
                {
                    Born = born,
                    Consented = consented,
                    Created = created,
                    EmailAddress = ea,
                    Forename = FORENAME,
                    Id = id,
                    IsActive = true,
                    Otp = OTP,
                    OtpExpires = DateTime.MaxValue,
                    Surname = SURNAME,
                    Updated = created,
                    UserName = USER_NAME
                };

            users._updateAction = _ => { };
            return users;
        };

        this._repository._saveAction = () => { };
        this._password._hashFunc = _ => String.Empty;

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
                EMAIL_ADDRESS,
                OTP,
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
