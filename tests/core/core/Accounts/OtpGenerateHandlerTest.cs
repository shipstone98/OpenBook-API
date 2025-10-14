using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Authentication;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Mail;

using Shipstone.OpenBook.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Accounts;

public sealed class OtpGenerateHandlerTest
{
    private readonly MockAuthenticationService _authentication;
    private readonly IOtpGenerateHandler _handler;
    private readonly MockMailService _mail;
    private readonly MockRepository _repository;

    public OtpGenerateHandlerTest()
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
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._authentication = authentication;
        this._handler = provider.GetRequiredService<IOtpGenerateHandler>();
        this._mail = mail;
        this._repository = repository;
    }

#region HandleAsync method
    [Fact]
    public async Task TestHandleAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("emailAddress", ex.ParamName);
    }

#region Valid arguments
    [Fact]
    public Task TestHandleAsync_Valid_Failure_EmailAddressNotFound()
    {
        // Arrange
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
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotActive()
    {
        // Arrange
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
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Success()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true
                };

            users._updateAction = _ => { };
            return users;
        };

        this._authentication._generateOtpAction = (u, n) =>
            u.OtpExpires = n.AddDays(1);

        this._repository._saveAction = () => { };
        this._mail._sendOtpAction = (_, _) => { };

        // Act
        return this._handler.HandleAsync(
            String.Empty,
            CancellationToken.None
        );

        // Nothing to assert
    }
#endregion
#endregion
}
