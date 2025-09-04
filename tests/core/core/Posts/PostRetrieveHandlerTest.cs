using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Posts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

using Shipstone.OpenBook.Api.CoreTest.Mocks;
using Shipstone.OpenBook.Api.Test.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Posts;

public sealed class PostRetrieveHandlerTest
{
    private readonly MockClaimsService _claims;
    private readonly IPostRetrieveHandler _handler;
    private readonly MockRepository _repository;

    public PostRetrieveHandlerTest()
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
        this._handler = provider.GetRequiredService<IPostRetrieveHandler>();
        this._repository = repository;
    }

#region HandleAsync method
    [InlineData(Int64.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [Theory]
    public async Task TestHandleAsync_Invalid(long id)
    {
        // Act
        ArgumentOutOfRangeException ex =
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                this._handler.HandleAsync(id, CancellationToken.None));

        // Assert
        Assert.Equal(id, ex.ActualValue);
        Assert.Equal("id", ex.ParamName);
    }

#region Valid arguments
    [Fact]
    public async Task TestHandleAsync_Valid_Failure_CreatorNotFound()
    {
        // Arrange
        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();
            posts._retrieveFunc = _ => new();
            return posts;
        };

        this._claims._isAuthenticatedFunc = () => false;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => null;
            return users;
        };

        // Act and assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(12345, CancellationToken.None));
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Failure_PostNotFound()
    {
        // Arrange
        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();
            posts._retrieveFunc = _ => null;
            return posts;
        };

        // Act and assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(1, CancellationToken.None));
    }

#region Success
    [Fact]
    public async Task TestHandleAsync_Valid_Success_Authenticated_Creator()
    {
#region Arrange
        // Arrange
        const long ID = 12345;
        DateTime created = DateTime.UnixEpoch.ToUniversalTime();
        DateTime updated = created.AddDays(12345);
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
                    ParentId = PARENT_ID,
                    Updated = updated
                };

            return posts;
        };

        this._claims._isAuthenticatedFunc = () => true;
        this._claims._idFunc = () => creatorId;
        this._claims._emailAddressFunc = () => CREATOR_EMAIL_ADDRESS;
        this._claims._userNameFunc = () => CREATOR_USER_NAME;
#endregion

        // Act
        IPost post =
            await this._handler.HandleAsync(ID, CancellationToken.None);

        // Assert
        post.AssertEqual(
            ID,
            created,
            updated,
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
        const long ID = 12345;
        DateTime created = DateTime.UnixEpoch.ToUniversalTime();
        DateTime updated = created.AddDays(12345);
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
                    ParentId = PARENT_ID,
                    Updated = updated
                };

            return posts;
        };

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
#endregion

        // Act
        IPost post =
            await this._handler.HandleAsync(ID, CancellationToken.None);

        // Assert
        post.AssertEqual(
            ID,
            created,
            updated,
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
        const long ID = 12345;
        DateTime created = DateTime.UnixEpoch.ToUniversalTime();
        DateTime updated = created.AddDays(12345);
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
                    CreatorId = Guid.NewGuid(),
                    Id = ID,
                    ParentId = PARENT_ID,
                    Updated = updated
                };

            return posts;
        };

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
#endregion

        // Act
        IPost post =
            await this._handler.HandleAsync(ID, CancellationToken.None);

        // Assert
        post.AssertEqual(
            ID,
            created,
            updated,
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
