using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Posts;
using Shipstone.OpenBook.Api.Infrastructure.Authorization;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

using Shipstone.OpenBook.Api.CoreTest.Mocks;
using Shipstone.OpenBook.Api.Test.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Posts;

public sealed class PostDeleteHandlerTest
{
    private readonly MockAuthorizationService _authorization;
    private readonly MockClaimsService _claims;
    private readonly IPostDeleteHandler _handler;
    private readonly MockRepository _repository;

    public PostDeleteHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookCore();
        MockAuthorizationService authorization = new();
        services.AddSingleton<IAuthorizationService>(authorization);
        MockClaimsService claims = new();
        services.AddSingleton<IClaimsService>(claims);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._authorization = authorization;
        this._claims = claims;
        this._handler = provider.GetRequiredService<IPostDeleteHandler>();
        this._repository = repository;
    }

#region HandleAsync method
    [InlineData(Int64.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [Theory]
    public async Task TestHandleAsync_Invalid_IdInvalid(long id)
    {
        // Act
        ArgumentOutOfRangeException ex =
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                this._handler.HandleAsync(
                    id,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal(id, ex.ActualValue);
        Assert.Equal("id", ex.ParamName);
    }

    [Fact]
    public async Task TestHandleAsync_Invalid_PolicyNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    12345,
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("policy", ex.ParamName);
    }

#region Valid arguments
#region Failure
    [Fact]
    public Task TestHandleAsync_Valid_Failure_CreatorNotFound()
    {
        // Arrange
        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();
            posts._retrieveFunc = _ => new();
            return posts;
        };

        this._authorization._authorizeAction = (_, _) => { };
        this._claims._isAuthenticatedFunc = () => false;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => null;
            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
                12345,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_PostNotFound()
    {
        // Arrange
        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();
            posts._retrieveFunc = _ => null;
            return posts;
        };

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
                12345,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Failure_UserNotAuthorized()
    {
        // Arrange
        Exception innerException = new ForbiddenException();

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();
            posts._retrieveFunc = _ => new();
            return posts;
        };

        this._authorization._authorizeAction = (_, _) => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<ForbiddenException>(() =>
                this._handler.HandleAsync(
                    12345,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.True(Object.ReferenceEquals(innerException, ex.InnerException));
    }
#endregion

#region Success
    [Fact]
    public async Task TestHandleAsync_Valid_Success_Authenticated_Creator()
    {
#region Arrange
        // Arrange
        const int ID = 12345;
        DateTime created = DateTime.UnixEpoch.ToUniversalTime();
        Guid creatorId = Guid.NewGuid();
        const String CREATOR_EMAIL_ADDRESS = "john.doe@contoso.com";
        const String CREATOR_USER_NAME = "johndoe2025";
        const long PARENT_ID = 67890;
        const String BODY = "Hello, world!";

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();

            posts._retrieveFunc = id =>
                new PostEntity
                {
                    Body = BODY,
                    Created = created,
                    CreatorId = creatorId,
                    Id = ID,
                    ParentId = PARENT_ID
                };

            posts._deleteAction = _ => { };
            return posts;
        };

        this._authorization._authorizeAction = (_, _) => { };
        this._claims._isAuthenticatedFunc = () => true;
        this._claims._idFunc = () => creatorId;
        this._claims._emailAddressFunc = () => CREATOR_EMAIL_ADDRESS;
        this._claims._userNameFunc = () => CREATOR_USER_NAME;
        this._repository._saveAction = () => { };
        DateTime notBefore = DateTime.UtcNow;
#endregion

        // Act
        IPost post =
            await this._handler.HandleAsync(
                12345,
                String.Empty,
                CancellationToken.None
            );

        // Assert
        Assert.False(DateTime.Compare(notBefore, post.Updated) > 0);

        post.AssertEqual(
            ID,
            created,
            post.Updated,
            CREATOR_EMAIL_ADDRESS,
            CREATOR_USER_NAME,
            BODY,
            PARENT_ID
        );
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Success_Authenticated_NotCreator()
    {
#region Arrange
        // Arrange
        const int ID = 12345;
        DateTime created = DateTime.UnixEpoch.ToUniversalTime();
        const String CREATOR_EMAIL_ADDRESS = "john.doe@contoso.com";
        const String CREATOR_USER_NAME = "johndoe2025";
        const long PARENT_ID = 67890;
        const String BODY = "Hello, world!";

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();

            posts._retrieveFunc = id =>
                new PostEntity
                {
                    Body = BODY,
                    Created = created,
                    Id = ID,
                    ParentId = PARENT_ID
                };

            posts._deleteAction = _ => { };
            return posts;
        };

        this._authorization._authorizeAction = (_, _) => { };
        this._claims._isAuthenticatedFunc = () => true;
        this._claims._idFunc = Guid.NewGuid;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = _ =>
                new UserEntity
                {
                    EmailAddress = CREATOR_EMAIL_ADDRESS,
                    UserName = CREATOR_USER_NAME
                };

            return users;
        };

        this._repository._saveAction = () => { };
        DateTime notBefore = DateTime.UtcNow;
#endregion

        // Act
        IPost post =
            await this._handler.HandleAsync(
                12345,
                String.Empty,
                CancellationToken.None
            );

        // Assert
        Assert.False(DateTime.Compare(notBefore, post.Updated) > 0);

        post.AssertEqual(
            ID,
            created,
            post.Updated,
            CREATOR_EMAIL_ADDRESS,
            CREATOR_USER_NAME,
            BODY,
            PARENT_ID
        );
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Success_NotAuthenticated()
    {
#region Arrange
        // Arrange
        const int ID = 12345;
        DateTime created = DateTime.UnixEpoch.ToUniversalTime();
        const String CREATOR_EMAIL_ADDRESS = "john.doe@contoso.com";
        const String CREATOR_USER_NAME = "johndoe2025";
        const long PARENT_ID = 67890;
        const String BODY = "Hello, world!";

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();

            posts._retrieveFunc = id =>
                new PostEntity
                {
                    Body = BODY,
                    Created = created,
                    Id = ID,
                    ParentId = PARENT_ID
                };

            posts._deleteAction = _ => { };
            return posts;
        };

        this._authorization._authorizeAction = (_, _) => { };
        this._claims._isAuthenticatedFunc = () => false;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = _ =>
                new UserEntity
                {
                    EmailAddress = CREATOR_EMAIL_ADDRESS,
                    UserName = CREATOR_USER_NAME
                };

            return users;
        };

        this._repository._saveAction = () => { };
        DateTime notBefore = DateTime.UtcNow;
#endregion

        // Act
        IPost post =
            await this._handler.HandleAsync(
                12345,
                String.Empty,
                CancellationToken.None
            );

        // Assert
        Assert.False(DateTime.Compare(notBefore, post.Updated) > 0);

        post.AssertEqual(
            ID,
            created,
            post.Updated,
            CREATOR_EMAIL_ADDRESS,
            CREATOR_USER_NAME,
            BODY,
            PARENT_ID
        );
    }
#endregion
#endregion
#endregion
}
