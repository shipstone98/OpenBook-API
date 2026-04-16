using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Extensions.Security;
using Shipstone.Utilities;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Mail;

using Shipstone.OpenBook.Api.CoreTest.Mocks;
using Shipstone.OpenBook.Api.Test.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Accounts;

public sealed class RegisterHandlerTest
{
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
        MockMailService mail = new();
        services.AddSingleton<IMailService>(mail);
        MockNormalizationService normalization = new();
        services.AddSingleton<INormalizationService>(normalization);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._handler = provider.GetRequiredService<IRegisterHandler>();
        this._mail = mail;
        this._normalization = normalization;
        this._repository = repository;
    }

#region HandleAsync method
#region Invalid arguments
    [Fact]
    public async Task TestHandleAsync_Invalid_IdentityIdInvalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._handler.HandleAsync(
                    Guid.Empty,
                    "johndoe2025",
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("identityId", ex.ParamName);
    }

    [Fact]
    public Task TestHandleAsync_Invalid_UserNameInvalid() =>
        throw new NotImplementedException();

    [Fact]
    public async Task TestHandleAsync_Invalid_UserNameNull()
    {
        // Arrange
        Guid identityId = Guid.NewGuid();

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    identityId,
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("userName", ex.ParamName);
    }
#endregion

#region Valid arguments
    [Fact]
    public async Task TestHandleAsync_Valid_Failure()
    {
        // Arrange
        Exception innerException = new();
        Guid identityId = Guid.NewGuid();
        this._normalization._normalizeFunc = _ => String.Empty;

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

        this._repository._saveAction = () => { };

        // Act
        Exception ex =
            await Assert.ThrowsAsync<ConflictException>(() =>
                this._handler.HandleAsync(
                    identityId,
                    "johndoe2025",
                    CancellationToken.None
                ));

        // Assert
        Assert.Same(innerException, ex.InnerException);
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Success()
    {
        // Arrange
        Guid identityId = Guid.NewGuid();
        Guid id = Guid.NewGuid();
        const String USER_NAME = "johndoe2025";
        this._normalization._normalizeFunc = _ => String.Empty;

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
        this._mail._sendRegistrationAction = () => { };
        DateTime notBefore = DateTime.UtcNow;

        // Act
        IUser user =
            await this._handler.HandleAsync(
                identityId,
                USER_NAME,
                CancellationToken.None
            );

        // Assert
        Assert.False(DateTime.Compare(notBefore, user.Created) > 0);
        IEnumerable<String> roles = new String[1] { Roles.User };

        user.AssertEqual(
            id,
            user.Created,
            user.Created,
            USER_NAME.Trim(),
            user.Created,
            roles
        );
    }
#endregion
#endregion
}
