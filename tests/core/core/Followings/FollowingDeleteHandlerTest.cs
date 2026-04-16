using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Utilities;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Followings;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Notifications;

using Shipstone.OpenBook.Api.CoreTest.Mocks;
using Shipstone.OpenBook.Api.Test.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Followings;

public sealed class FollowingDeleteHandlerTest
{
    private readonly MockClaimsService _claims;
    private readonly IFollowingDeleteHandler _handler;
    private readonly MockNotificationService _notification;
    private readonly MockRepository _repository;

    public FollowingDeleteHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookCore();
        MockClaimsService claims = new();
        services.AddSingleton<IClaimsService>(claims);
        MockNotificationService notification = new();
        services.AddSingleton<INotificationService>(notification);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._claims = claims;
        this._handler = provider.GetRequiredService<IFollowingDeleteHandler>();
        this._notification = notification;
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
                    TestContext.Current.CancellationToken
                ));

        // Assert
        Assert.Equal("userName", ex.ParamName);
    }

#region Valid arguments
#region Failure
    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserCurrentUser()
    {
#region Arrange
        // Arrange
        Guid id = Guid.NewGuid();

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    Id = id,
                    IsActive = true
                };

            return users;
        };

        this._claims._userFunc = () =>
        {
            MockUser user = new();
            user._idFunc = () => id;
            return user;
        };

        this._repository._userFollowingsFunc = () =>
        {
            MockUserFollowingRepository userFollowings = new();
            userFollowings._retrieveFunc = (_, _) => null;
            return userFollowings;
        };
#endregion

        // Act
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                TestContext.Current.CancellationToken
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

        // Act
        return Assert.ThrowsAsync<UserNotActiveException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                TestContext.Current.CancellationToken
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotFollowed()
    {
#region Arrange
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true
                };

            return users;
        };

        this._claims._userFunc = () =>
        {
            MockUser user = new();
            user._idFunc = Guid.NewGuid;
            return user;
        };

        this._repository._userFollowingsFunc = () =>
        {
            MockUserFollowingRepository userFollowings = new();
            userFollowings._retrieveFunc = (_, _) => null;
            return userFollowings;
        };
#endregion

        // Act
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                TestContext.Current.CancellationToken
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotFound()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_StringFunc = _ => null;
            return users;
        };

        // Act
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                TestContext.Current.CancellationToken
            ));
    }
#endregion

    [InlineData(false)]
    [InlineData(true)]
    [Theory]
    public async Task TestHandleAsync_Valid_Success(bool isSubscribed)
    {
#region Arrange
        // Arrange
        const String FOLLOWER_NAME = "johndoe2025";
        const String FOLLOWEE_NAME = "janedoe2025";
        DateTime followed = DateTime.UnixEpoch.ToUniversalTime();

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = un =>
                new UserEntity
                {
                    IsActive = true,
                    UserName = un
                };

            return users;
        };

        this._claims._userFunc = () =>
        {
            MockUser user = new();
            user._idFunc = Guid.NewGuid;
            user._userNameFunc = () => FOLLOWER_NAME;
            return user;
        };

        this._repository._userFollowingsFunc = () =>
        {
            MockUserFollowingRepository userFollowings = new();

            userFollowings._retrieveFunc = (_, _) =>
                new UserFollowingEntity
                {
                   Followed = followed,
                   IsSubscribed = isSubscribed
                };

            userFollowings._deleteAction = _ => { };
            return userFollowings;
        };

        this._repository._saveAction = () => { };

        this._repository._userDevicesFunc = () =>
        {
            MockUserDeviceRepository userDevices = new();

            userDevices._listForUserFunc = _ => new UserDeviceEntity[]
            {
                new UserDeviceEntity { },
                new UserDeviceEntity { }
            };

            return userDevices;
        };

        this._notification._sendUserUnfollowedAction = (_, _) => { };
#endregion

        // Act
        IFollowing following =
            await this._handler.HandleAsync(
                FOLLOWEE_NAME,
                TestContext.Current.CancellationToken
            );

        // Assert
        following.AssertEqual(
            FOLLOWER_NAME,
            FOLLOWEE_NAME,
            followed,
            isSubscribed
        );
    }
#endregion
#endregion
}
