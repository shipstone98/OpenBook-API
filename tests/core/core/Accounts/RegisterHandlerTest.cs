using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Infrastructure.Authentication;
using Shipstone.OpenBook.Api.Infrastructure.Data;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Mail;

using Shipstone.OpenBook.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Accounts;

public sealed class RegisterHandlerTest
{
    private readonly MockAuthenticationService _authentication;
    private readonly IRegisterHandler _handler;
    private readonly MockMailService _mail;
    private readonly MockNormalizationService _normalization;
    private readonly MockRepository _repository;

    public RegisterHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookCore();
        MockAuthenticationService authentication = new();
        services.AddSingleton<IAuthenticationService>(authentication);
        MockMailService mail = new();
        services.AddSingleton<IMailService>(mail);
        MockNormalizationService normalization = new();
        services.AddSingleton<INormalizationService>(normalization);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._authentication = authentication;
        this._handler = provider.GetRequiredService<IRegisterHandler>();
        this._mail = mail;
        this._normalization = normalization;
        this._repository = repository;
    }

#region HandleAsync method
#region Invalid arguments
    [Fact]
    public async Task TestHandleAsync_Invalid_BornInvalid_InvalidDate()
    {
        // Arrange
        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddDays(1);

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._handler.HandleAsync(
                    "john.doe@contoso.com",
                    "johndoe2025",
                    "John",
                    "Doe",
                    born,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("born", ex.ParamName);
    }

    [Fact]
    public async Task TestHandleAsync_Invalid_BornInvalid_ValidDate()
    {
        // Arrange
        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18)
                .AddDays(1);

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._handler.HandleAsync(
                    "john.doe@contoso.com",
                    "johndoe2025",
                    "John",
                    "Doe",
                    born,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("born", ex.ParamName);
    }

    [Fact]
    public Task TestHandleAsync_Invalid_EmailAddressInvalid() => throw new NotImplementedException();

    [Fact]
    public async Task TestHandleAsync_Invalid_EmailAddressNull()
    {
        // Arrange
        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18);

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    null!,
                    "johndoe2025",
                    "John",
                    "Doe",
                    born,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("emailAddress", ex.ParamName);
    }

    [Fact]
    public Task TestHandleAsync_Invalid_ForenameInvalid() => throw new NotImplementedException();

    [Fact]
    public async Task TestHandleAsync_Invalid_ForenameNull()
    {
        // Arrange
        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18);

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    "john.doe@contoso.com",
                    "johndoe2025",
                    null!,
                    "Doe",
                    born,
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
        // Arrange
        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18);

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    "john.doe@contoso.com",
                    "johndoe2025",
                    "John",
                    null!,
                    born,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("surname", ex.ParamName);
    }

    [Fact]
    public Task TestHandleAsync_Invalid_UserNameInvalid() => throw new NotImplementedException();

    [Fact]
    public async Task TestHandleAsync_Invalid_UserNameNull()
    {
        // Arrange
        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18);

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    "john.doe@contoso.com",
                    null!,
                    "John",
                    "Doe",
                    born,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("userName", ex.ParamName);
    }
#endregion

    [Fact]
    public async Task TestHandleAsync_Valid_Failure()
    {
        // Arrange
        Exception innerException = new();

        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18);

        this._normalization._normalizeFunc = _ => String.Empty;
        this._authentication._generateOtpAction = (_, _) => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._createAction = _ => { };
            return users;
        };

        this._repository._userRolesFunc = () =>
        {
            MockUserRoleRepository userRoles = new();
            userRoles._createAction = _ => { };
            return userRoles;
        };

        this._repository._saveAction = () => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<ConflictException>(() =>
                this._handler.HandleAsync(
                    "john.doe@contoso.com",
                    "johndoe2025",
                    "John",
                    "Doe",
                    born,
                    CancellationToken.None
                ));

        // Assert
        Assert.Same(innerException, ex.InnerException);
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Success()
    {
#region Arrange
        // Arrange
        Guid id = Guid.NewGuid();
        const String EMAIL_ADDRESS = "john.doe@contoso.com";
        const String USER_NAME = "johndoe2025";
        const String FORENAME = "John";
        const String SURNAME = "Doe";

        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18);

        this._normalization._normalizeFunc = _ => String.Empty;

        this._authentication._generateOtpAction = (u, _) =>
            u.OtpExpires = DateTime.MaxValue;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._createAction = u => u.SetId(id);
            return users;
        };

        this._repository._userRolesFunc = () =>
        {
            MockUserRoleRepository userRoles = new();
            userRoles._createAction = _ => { };
            return userRoles;
        };

        this._repository._saveAction = () => { };
        this._mail._sendRegistrationAction = (_, _) => { };
        DateTime notBefore = DateTime.UtcNow;
#endregion

        // Act
        IUser user =
            await this._handler.HandleAsync(
                EMAIL_ADDRESS,
                USER_NAME,
                FORENAME,
                SURNAME,
                born,
                CancellationToken.None
            );

        // Assert
        Assert.False(DateTime.Compare(notBefore, user.Created) > 0);
        IEnumerable<String> roles = new String[1] { Roles.User };

        user.AssertEqual(
            id,
            user.Created,
            user.Created,
            EMAIL_ADDRESS,
            USER_NAME,
            FORENAME,
            SURNAME,
            born,
            user.Created,
            roles
        );
    }
#endregion
}
