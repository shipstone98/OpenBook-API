using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Followings;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

using Shipstone.OpenBook.Api.CoreTest.Mocks;
using Shipstone.OpenBook.Api.Test.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Followings;

public sealed class FollowingRetrieveHandlerTest
{
    private readonly MockClaimsService _claims;
    private readonly IFollowingRetrieveHandler _handler;
    private readonly MockRepository _repository;

    public FollowingRetrieveHandlerTest()
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
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._claims = claims;
        this._handler = provider.GetRequiredService<IFollowingRetrieveHandler>();
        this._repository = repository;
    }

#region HandleAsync method
    [Fact]
    public async Task TestHandleAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("userName", ex.ParamName);
    }

#region Valid arguments
#region Failure
    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserCurrentUser()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieveForNameFunc = _ =>
                new UserEntity
                {
                    Id = id,
                    IsActive = true
                };

            return users;
        };

        this._claims._idFunc = () => id;

        this._repository._userFollowingsFunc = () =>
        {
            MockUserFollowingRepository userFollowings = new();
            userFollowings._retrieveFunc = (_, _) => null;
            return userFollowings;
        };

        // Act
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(String.Empty, CancellationToken.None));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotActive()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieveForNameFunc = _ => new();
            return users;
        };

        // Act
        return Assert.ThrowsAsync<UserNotActiveException>(() =>
            this._handler.HandleAsync(String.Empty, CancellationToken.None));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotFollowed()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieveForNameFunc = _ =>
                new UserEntity
                {
                    IsActive = true
                };

            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        this._repository._userFollowingsFunc = () =>
        {
            MockUserFollowingRepository userFollowings = new();
            userFollowings._retrieveFunc = (_, _) => null;
            return userFollowings;
        };

        // Act
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(String.Empty, CancellationToken.None));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotFound()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieveForNameFunc = _ => null;
            return users;
        };

        // Act
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(String.Empty, CancellationToken.None));
    }
#endregion

    [InlineData(false)]
    [InlineData(true)]
    [Theory]
    public async Task TestHandleAsync_Valid_Success(bool isSubscribed)
    {
#region Arrange
        // Arrange
        const String FOLLOWER_EMAIL_ADDRESS = "john.doe@contoso.com";
        const String FOLLOWEE_NAME = "janedoe2025";
        DateTime followed = DateTime.UnixEpoch.ToUniversalTime();

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieveForNameFunc = un =>
                new UserEntity
                {
                    IsActive = true,
                    UserName = un
                };

            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        this._repository._userFollowingsFunc = () =>
        {
            MockUserFollowingRepository userFollowings = new();

            userFollowings._retrieveFunc = (_, _) =>
                new UserFollowingEntity
                {
                    Followed = followed,
                    IsSubscribed = isSubscribed
                };

            return userFollowings;
        };

        this._claims._emailAddressFunc = () => FOLLOWER_EMAIL_ADDRESS;
#endregion

        // Act
        IFollowing following =
            await this._handler.HandleAsync(
                FOLLOWEE_NAME,
                CancellationToken.None
            );

        // Assert
        following.AssertEqual(
            following.FollowerEmailAddress,
            following.FolloweeName,
            following.Followed,
            following.IsSubscribed
        );
    }
#endregion
#endregion
}
