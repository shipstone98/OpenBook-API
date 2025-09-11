using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Utilities.Collections;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Posts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

using Shipstone.OpenBook.Api.CoreTest.Mocks;
using Shipstone.OpenBook.Api.Test.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Posts;

public sealed class PostListHandlerTest
{
    private readonly MockClaimsService _claims;
    private readonly IPostListHandler _handler;
    private readonly MockRepository _repository;

    public PostListHandlerTest()
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
        this._handler = provider.GetRequiredService<IPostListHandler>();
        this._repository = repository;
    }

#region HandleAsync methods
    [Fact]
    public async Task TestHandleAsync_Empty()
    {
        // Arrange
        const int TOTAL_COUNT = 0;
        const int PAGE_INDEX = 0;
        const int PAGE_COUNT = 1;
        this._claims._idFunc = Guid.NewGuid;
        this._claims._emailAddressFunc = () => "john.doe@contoso.com";
        this._claims._userNameFunc = () => "johndoe2025";

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();

            posts._listForCreatorFunc = _ =>
            {
                MockReadOnlyPaginatedList<PostEntity> posts = new();

                IEnumerable<PostEntity> postCollection =
                    Array.Empty<PostEntity>();

                posts._getEnumeratorFunc = postCollection.GetEnumerator;
                posts._totalCountFunc = () => TOTAL_COUNT;
                posts._pageIndexFunc = () => PAGE_INDEX;
                posts._pageCountFunc = () => PAGE_COUNT;
                return posts;
            };

            return posts;
        };

        // Act
        IReadOnlyPaginatedList<IPost> posts =
            await this._handler.HandleAsync(CancellationToken.None);

        // Assert
        Assert.Empty(posts);
        Assert.Equal(PAGE_COUNT, posts.PageCount);
        Assert.Equal(PAGE_INDEX, posts.PageIndex);
        Assert.Equal(TOTAL_COUNT, posts.TotalCount);
    }

    [Fact]
    public async Task TestHandleAsync_NotEmpty()
    {
#region Arrange
        // Arrange
        const long ID_1 = 123;
        const long ID_2 = 345;
        DateTime created1 = DateTime.UnixEpoch.ToUniversalTime();
        DateTime created2 = created1.AddDays(10);
        DateTime updated1 = created1.AddDays(12345);
        DateTime updated2 = created2.AddDays(12345);
        const String CREATOR_EMAIL_ADDRESS = "john.doe@contoso.com";
        const String CREATOR_USER_NAME = "johndoe2025";
        const long PARENT_ID_1 = 678;
        const long PARENT_ID_2 = 890;
        const String BODY_1 = "Hello, world!";
        const String BODY_2 = "Hello, world! This is a second post.";
        const int TOTAL_COUNT = 12345;
        const int PAGE_INDEX = 17;
        const int PAGE_COUNT = 31;
        const int COUNT = 2;
        this._claims._idFunc = Guid.NewGuid;
        this._claims._emailAddressFunc = () => CREATOR_EMAIL_ADDRESS;
        this._claims._userNameFunc = () => CREATOR_USER_NAME;

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();

            posts._listForCreatorFunc = _ =>
            {
                MockReadOnlyPaginatedList<PostEntity> posts = new();

                IEnumerable<PostEntity> postCollection = new PostEntity[COUNT]
                {
                    new PostEntity
                    {
                        Body = BODY_1,
                        Created = created1,
                        Id = ID_1,
                        ParentId = PARENT_ID_1,
                        Updated = updated1
                    },
                    new PostEntity
                    {
                        Body = BODY_2,
                        Created = created2,
                        Id = ID_2,
                        ParentId = PARENT_ID_2,
                        Updated = updated2
                    }
                };

                posts._getEnumeratorFunc = postCollection.GetEnumerator;
                posts._totalCountFunc = () => TOTAL_COUNT;
                posts._pageIndexFunc = () => PAGE_INDEX;
                posts._pageCountFunc = () => PAGE_COUNT;
                return posts;
            };

            return posts;
        };
#endregion

        // Act
        IReadOnlyPaginatedList<IPost> posts =
            await this._handler.HandleAsync(CancellationToken.None);

        // Assert
        Assert.Equal(COUNT, posts.Count);
        Assert.Equal(PAGE_COUNT, posts.PageCount);
        Assert.Equal(PAGE_INDEX, posts.PageIndex);
        Assert.Equal(TOTAL_COUNT, posts.TotalCount);

        posts[0].AssertEqual(
            ID_1,
            created1,
            updated1,
            CREATOR_EMAIL_ADDRESS,
            CREATOR_USER_NAME,
            BODY_1,
            PARENT_ID_1
        );

        posts[1].AssertEqual(
            ID_2,
            created2,
            updated2,
            CREATOR_EMAIL_ADDRESS,
            CREATOR_USER_NAME,
            BODY_2,
            PARENT_ID_2
        );
    }

#region String parameter
    [Fact]
    public async Task TestHandleAsync_String_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("userName", ex.ParamName);
    }

#region Valid arguments
    [Fact]
    public Task TestHandleAsync_String_Valid_Failure()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieveForNameFunc = _ => null;
            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(String.Empty, CancellationToken.None));
    }

    [Fact]
    public async Task TestHandleAsync_String_Valid_Success_Empty()
    {
        // Arrange
        const int TOTAL_COUNT = 0;
        const int PAGE_INDEX = 0;
        const int PAGE_COUNT = 1;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieveForNameFunc = _ => new();
            return users;
        };

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();

            posts._listForCreatorFunc = _ =>
            {
                MockReadOnlyPaginatedList<PostEntity> posts = new();

                IEnumerable<PostEntity> postCollection =
                    Array.Empty<PostEntity>();

                posts._getEnumeratorFunc = postCollection.GetEnumerator;
                posts._totalCountFunc = () => TOTAL_COUNT;
                posts._pageIndexFunc = () => PAGE_INDEX;
                posts._pageCountFunc = () => PAGE_COUNT;
                return posts;
            };

            return posts;
        };

        // Act
        IReadOnlyPaginatedList<IPost> posts =
            await this._handler.HandleAsync(
                String.Empty,
                CancellationToken.None
            );

        // Assert
        Assert.Empty(posts);
        Assert.Equal(PAGE_COUNT, posts.PageCount);
        Assert.Equal(PAGE_INDEX, posts.PageIndex);
        Assert.Equal(TOTAL_COUNT, posts.TotalCount);
    }

    [Fact]
    public async Task TestHandleAsync_String_Valid_Success_NotEmpty()
    {
#region Arrange
        // Arrange
        const long ID_1 = 123;
        const long ID_2 = 345;
        DateTime created1 = DateTime.UnixEpoch.ToUniversalTime();
        DateTime created2 = created1.AddDays(10);
        DateTime updated1 = created1.AddDays(12345);
        DateTime updated2 = created2.AddDays(12345);
        const String CREATOR_EMAIL_ADDRESS = "john.doe@contoso.com";
        const String CREATOR_USER_NAME = "johndoe2025";
        const long PARENT_ID_1 = 678;
        const long PARENT_ID_2 = 890;
        const String BODY_1 = "Hello, world!";
        const String BODY_2 = "Hello, world! This is a second post.";
        const int TOTAL_COUNT = 12345;
        const int PAGE_INDEX = 17;
        const int PAGE_COUNT = 31;
        const int COUNT = 2;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieveForNameFunc = _ =>
                new UserEntity
                {
                    EmailAddress = CREATOR_EMAIL_ADDRESS,
                    UserName = CREATOR_USER_NAME
                };

            return users;
        };

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();

            posts._listForCreatorFunc = _ =>
            {
                MockReadOnlyPaginatedList<PostEntity> posts = new();

                IEnumerable<PostEntity> postCollection = new PostEntity[COUNT]
                {
                    new PostEntity
                    {
                        Body = BODY_1,
                        Created = created1,
                        Id = ID_1,
                        ParentId = PARENT_ID_1,
                        Updated = updated1
                    },
                    new PostEntity
                    {
                        Body = BODY_2,
                        Created = created2,
                        Id = ID_2,
                        ParentId = PARENT_ID_2,
                        Updated = updated2
                    }
                };

                posts._getEnumeratorFunc = postCollection.GetEnumerator;
                posts._totalCountFunc = () => TOTAL_COUNT;
                posts._pageIndexFunc = () => PAGE_INDEX;
                posts._pageCountFunc = () => PAGE_COUNT;
                return posts;
            };

            return posts;
        };
#endregion

        // Act
        IReadOnlyPaginatedList<IPost> posts =
            await this._handler.HandleAsync(
                String.Empty,
                CancellationToken.None
            );

        // Assert
        Assert.Equal(COUNT, posts.Count);
        Assert.Equal(PAGE_COUNT, posts.PageCount);
        Assert.Equal(PAGE_INDEX, posts.PageIndex);
        Assert.Equal(TOTAL_COUNT, posts.TotalCount);

        posts[0].AssertEqual(
            ID_1,
            created1,
            updated1,
            CREATOR_EMAIL_ADDRESS,
            CREATOR_USER_NAME,
            BODY_1,
            PARENT_ID_1
        );

        posts[1].AssertEqual(
            ID_2,
            created2,
            updated2,
            CREATOR_EMAIL_ADDRESS,
            CREATOR_USER_NAME,
            BODY_2,
            PARENT_ID_2
        );
    }
#endregion
#endregion
#endregion
}
