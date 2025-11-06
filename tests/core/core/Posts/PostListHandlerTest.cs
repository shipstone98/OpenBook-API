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
#region Int64 parameter
    [InlineData(Int64.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [Theory]
    public async Task TestHandleAsync_Int64_Invalid(long parentId)
    {
        // Act
        ArgumentOutOfRangeException ex =
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                this._handler.HandleAsync(parentId, CancellationToken.None));

        // Assert
        Assert.Equal(parentId, ex.ActualValue);
        Assert.Equal("parentId", ex.ParamName);
    }

#region Valid arguments
    [Fact]
    public Task TestHandleAsync_Int64_Valid_Failure()
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
            this._handler.HandleAsync(12345, CancellationToken.None));
    }

#region Success
    [Fact]
    public async Task TestHandleAsync_Int64_Valid_Success_Empty()
    {
        // Arrange
        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();
            posts._retrieveFunc = _ => new PostEntity();

            posts._listForParentFunc = _ =>
            {
                MockReadOnlyPaginatedList<PostEntity> posts = new();

                IEnumerable<PostEntity> postCollection =
                    Array.Empty<PostEntity>();

                posts._getEnumeratorFunc = postCollection.GetEnumerator;
                posts._totalCountFunc = () => 0;
                posts._pageIndexFunc = () => 0;
                posts._pageCountFunc = () => 1;
                return posts;
            };

            return posts;
        };

        // Act
        IReadOnlyPaginatedList<IPost> posts =
            await this._handler.HandleAsync(12345, CancellationToken.None);

        // Assert
        posts.AssertEmpty();
    }

#region Not empty
    [Fact]
    public async Task TestHandleAsync_Int64_Valid_Success_NotEmpty_Authenticated_Creator()
    {
#region Arrange
        // Arrange
        const long ID_1 = 123;
        const long ID_2 = 345;
        DateTime created1 = DateTime.UnixEpoch.ToUniversalTime();
        DateTime updated1 = created1.AddDays(10);
        DateTime created2 = created1.AddYears(10);
        DateTime updated2 = created2.AddDays(10);
        Guid creatorId1 = Guid.NewGuid();
        Guid creatorId2 = Guid.NewGuid();
        const String CREATOR_EMAIL_ADDRESS_1 = "john.doe@contoso.com";
        const String CREATOR_EMAIL_ADDRESS_2 = "jane.doe@contoso.com";
        const String CREATOR_USER_NAME_1 = "johndoe2025";
        const String CREATOR_USER_NAME_2 = "janedoe2025";
        const long PARENT_ID = 12345;
        const String BODY_1 = "My first post body.";
        const String BODY_2 = "My second post body.";
        const int TOTAL_COUNT = 31;
        const int PAGE_INDEX = 17;
        const int PAGE_COUNT = 23;
        const int COUNT = 2;

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();
            posts._retrieveFunc = _ => new PostEntity();

            posts._listForParentFunc = _ =>
            {
                MockReadOnlyPaginatedList<PostEntity> posts = new();

                IEnumerable<PostEntity> postCollection = new PostEntity[COUNT]
                {
                    new PostEntity
                    {
                        Body = BODY_1,
                        Created = created1,
                        CreatorId = creatorId1,
                        Id = ID_1,
                        ParentId = PARENT_ID,
                        Updated = updated1
                    },
                    new PostEntity
                    {
                        Body = BODY_2,
                        Created = created2,
                        CreatorId = creatorId2,
                        Id = ID_2,
                        ParentId = PARENT_ID,
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

        this._claims._isAuthenticatedFunc = () => true;
        this._claims._idFunc = () => creatorId1;
        this._claims._emailAddressFunc = () => CREATOR_EMAIL_ADDRESS_1;
        this._claims._userNameFunc = () => CREATOR_USER_NAME_1;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = _ =>
                new UserEntity
                {
                    EmailAddress = CREATOR_EMAIL_ADDRESS_2,
                    UserName = CREATOR_USER_NAME_2
                };

            return users;
        };
#endregion

        // Act
        IReadOnlyPaginatedList<IPost> posts =
            await this._handler.HandleAsync(PARENT_ID, CancellationToken.None);

        // Assert
        posts.AssertEqual(COUNT, TOTAL_COUNT, PAGE_INDEX, PAGE_COUNT);

        posts[0].AssertEqual(
            ID_1,
            created1,
            updated1,
            CREATOR_EMAIL_ADDRESS_1,
            CREATOR_USER_NAME_1,
            BODY_1,
            PARENT_ID
        );

        posts[1].AssertEqual(
            ID_2,
            created2,
            updated2,
            CREATOR_EMAIL_ADDRESS_2,
            CREATOR_USER_NAME_2,
            BODY_2,
            PARENT_ID
        );
    }

    [Fact]
    public async Task TestHandleAsync_Int64_Valid_Success_NotEmpty_Authenticated_NotCreator()
    {
#region Arrange
        // Arrange
        const long ID_1 = 123;
        const long ID_2 = 345;
        DateTime created1 = DateTime.UnixEpoch.ToUniversalTime();
        DateTime updated1 = created1.AddDays(10);
        DateTime created2 = created1.AddYears(10);
        DateTime updated2 = created2.AddDays(10);
        Guid creatorId1 = Guid.NewGuid();
        Guid creatorId2 = Guid.NewGuid();
        const String CREATOR_EMAIL_ADDRESS_1 = "john.doe@contoso.com";
        const String CREATOR_EMAIL_ADDRESS_2 = "jane.doe@contoso.com";
        const String CREATOR_USER_NAME_1 = "johndoe2025";
        const String CREATOR_USER_NAME_2 = "janedoe2025";
        const long PARENT_ID = 12345;
        const String BODY_1 = "My first post body.";
        const String BODY_2 = "My second post body.";
        const int TOTAL_COUNT = 31;
        const int PAGE_INDEX = 17;
        const int PAGE_COUNT = 23;
        const int COUNT = 2;

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();
            posts._retrieveFunc = _ => new PostEntity();

            posts._listForParentFunc = _ =>
            {
                MockReadOnlyPaginatedList<PostEntity> posts = new();

                IEnumerable<PostEntity> postCollection = new PostEntity[COUNT]
                {
                    new PostEntity
                    {
                        Body = BODY_1,
                        Created = created1,
                        CreatorId = creatorId1,
                        Id = ID_1,
                        ParentId = PARENT_ID,
                        Updated = updated1
                    },
                    new PostEntity
                    {
                        Body = BODY_2,
                        Created = created2,
                        CreatorId = creatorId2,
                        Id = ID_2,
                        ParentId = PARENT_ID,
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

        this._claims._isAuthenticatedFunc = () => true;
        this._claims._idFunc = Guid.NewGuid;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = id =>
            {
                if (Guid.Equals(id, creatorId1))
                {
                    return new UserEntity
                    {
                        EmailAddress = CREATOR_EMAIL_ADDRESS_1,
                        UserName = CREATOR_USER_NAME_1
                    };
                }

                return Guid.Equals(id, creatorId2)
                    ? new UserEntity
                    {
                        EmailAddress = CREATOR_EMAIL_ADDRESS_2,
                        UserName = CREATOR_USER_NAME_2
                    }
                    : null;
            };

            return users;
        };
#endregion

        // Act
        IReadOnlyPaginatedList<IPost> posts =
            await this._handler.HandleAsync(PARENT_ID, CancellationToken.None);

        // Assert
        posts.AssertEqual(COUNT, TOTAL_COUNT, PAGE_INDEX, PAGE_COUNT);

        posts[0].AssertEqual(
            ID_1,
            created1,
            updated1,
            CREATOR_EMAIL_ADDRESS_1,
            CREATOR_USER_NAME_1,
            BODY_1,
            PARENT_ID
        );

        posts[1].AssertEqual(
            ID_2,
            created2,
            updated2,
            CREATOR_EMAIL_ADDRESS_2,
            CREATOR_USER_NAME_2,
            BODY_2,
            PARENT_ID
        );
    }

    [Fact]
    public async Task TestHandleAsync_Int64_Valid_Success_NotEmpty_NotAuthenticated()
    {
#region Arrange
        // Arrange
        const long ID_1 = 123;
        const long ID_2 = 345;
        DateTime created1 = DateTime.UnixEpoch.ToUniversalTime();
        DateTime updated1 = created1.AddDays(10);
        DateTime created2 = created1.AddYears(10);
        DateTime updated2 = created2.AddDays(10);
        Guid creatorId1 = Guid.NewGuid();
        Guid creatorId2 = Guid.NewGuid();
        const String CREATOR_EMAIL_ADDRESS_1 = "john.doe@contoso.com";
        const String CREATOR_EMAIL_ADDRESS_2 = "jane.doe@contoso.com";
        const String CREATOR_USER_NAME_1 = "johndoe2025";
        const String CREATOR_USER_NAME_2 = "janedoe2025";
        const long PARENT_ID = 12345;
        const String BODY_1 = "My first post body.";
        const String BODY_2 = "My second post body.";
        const int TOTAL_COUNT = 31;
        const int PAGE_INDEX = 17;
        const int PAGE_COUNT = 23;
        const int COUNT = 2;

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();
            posts._retrieveFunc = _ => new PostEntity();

            posts._listForParentFunc = _ =>
            {
                MockReadOnlyPaginatedList<PostEntity> posts = new();

                IEnumerable<PostEntity> postCollection = new PostEntity[COUNT]
                {
                    new PostEntity
                    {
                        Body = BODY_1,
                        Created = created1,
                        CreatorId = creatorId1,
                        Id = ID_1,
                        ParentId = PARENT_ID,
                        Updated = updated1
                    },
                    new PostEntity
                    {
                        Body = BODY_2,
                        Created = created2,
                        CreatorId = creatorId2,
                        Id = ID_2,
                        ParentId = PARENT_ID,
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

        this._claims._isAuthenticatedFunc = () => false;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = id =>
            {
                if (Guid.Equals(id, creatorId1))
                {
                    return new UserEntity
                    {
                        EmailAddress = CREATOR_EMAIL_ADDRESS_1,
                        UserName = CREATOR_USER_NAME_1
                    };
                }

                return Guid.Equals(id, creatorId2)
                    ? new UserEntity
                    {
                        EmailAddress = CREATOR_EMAIL_ADDRESS_2,
                        UserName = CREATOR_USER_NAME_2
                    }
                    : null;
            };

            return users;
        };
#endregion

        // Act
        IReadOnlyPaginatedList<IPost> posts =
            await this._handler.HandleAsync(PARENT_ID, CancellationToken.None);

        // Assert
        posts.AssertEqual(COUNT, TOTAL_COUNT, PAGE_INDEX, PAGE_COUNT);

        posts[0].AssertEqual(
            ID_1,
            created1,
            updated1,
            CREATOR_EMAIL_ADDRESS_1,
            CREATOR_USER_NAME_1,
            BODY_1,
            PARENT_ID
        );

        posts[1].AssertEqual(
            ID_2,
            created2,
            updated2,
            CREATOR_EMAIL_ADDRESS_2,
            CREATOR_USER_NAME_2,
            BODY_2,
            PARENT_ID
        );
    }
#endregion
#endregion
#endregion
#endregion

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
        posts.AssertEqual(COUNT, TOTAL_COUNT, PAGE_INDEX, PAGE_COUNT);

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
