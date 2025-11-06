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

public sealed class PostAggregateHandlerTest
{
    private readonly MockClaimsService _claims;
    private readonly IPostAggregateHandler _handler;
    private readonly MockRepository _repository;

    public PostAggregateHandlerTest()
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
        this._handler = provider.GetRequiredService<IPostAggregateHandler>();
        this._repository = repository;
    }

    [Fact]
    public async Task TestHandleAsync_Empty()
    {
        // Arrange
        this._repository._userFollowingsFunc = () =>
        {
            MockUserFollowingRepository userFollowings = new();

            userFollowings._listForFollowerFunc = _ =>
                Array.Empty<UserFollowingEntity>();

            return userFollowings;
        };

        this._claims._idFunc = Guid.NewGuid;

        // Act
        IReadOnlyPaginatedList<IPost> posts =
            await this._handler.HandleAsync(CancellationToken.None);

        // Assert
        posts.AssertEmpty();
    }

    [Fact]
    public async Task TestHandleAsync_NotEmpty()
    {
#region Arrange
        // Arrange
        const int TOTAL_COUNT = 31;
        const int PAGE_INDEX = 1;
        const int PAGE_COUNT = 17;
        const int COUNT = 2;
        Guid followee1Id = Guid.NewGuid();
        const String FOLLOWEE_1_EMAIL_ADDRESS = "john.doe@contoso.com";
        const String FOLLOWEE_1_NAME = "johndoe2025";
        Guid followee2Id = Guid.NewGuid();
        const String FOLLOWEE_2_EMAIL_ADDRESS = "jane.doe@contoso.com";
        const String FOLLOWEE_2_NAME = "janedoe2025";

        this._repository._userFollowingsFunc = () =>
        {
            MockUserFollowingRepository userFollowings = new();

            userFollowings._listForFollowerFunc = _ =>
                new UserFollowingEntity[]
                {
                    new UserFollowingEntity
                    {
                        FolloweeId = followee1Id
                    },
                    new UserFollowingEntity
                    {
                        FolloweeId = followee2Id
                    }
                };

            return userFollowings;
        };

        this._claims._idFunc = Guid.NewGuid;

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();

            posts._listForCreatorsFunc = _ =>
            {
                IEnumerable<PostEntity> collection = new PostEntity[COUNT]
                {
                    new PostEntity
                    {
                        CreatorId = followee1Id,
                        Id = 1
                    },
                    new PostEntity
                    {
                        CreatorId = followee2Id,
                        Id = 2
                    }
                };

                MockReadOnlyPaginatedList<PostEntity> posts = new();
                posts._getEnumeratorFunc = collection.GetEnumerator;
                posts._totalCountFunc = () => TOTAL_COUNT;
                posts._pageIndexFunc = () => PAGE_INDEX;
                posts._pageCountFunc = () => PAGE_COUNT;
                return posts;
            };

            return posts;
        };

        this._claims._isAuthenticatedFunc = () => true;

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = id =>
            {
                if (Guid.Equals(id, followee1Id))
                {
                    return new UserEntity
                    {
                        EmailAddress = FOLLOWEE_1_EMAIL_ADDRESS,
                        UserName = FOLLOWEE_1_NAME
                    };
                }

                if (Guid.Equals(id, followee2Id))
                {
                    return new UserEntity
                    {
                        EmailAddress = FOLLOWEE_2_EMAIL_ADDRESS,
                        UserName = FOLLOWEE_2_NAME
                    };
                }

                return null;
            };

            return users;
        };
#endregion

        // Act
        IReadOnlyPaginatedList<IPost> posts =
            await this._handler.HandleAsync(CancellationToken.None);

        // Assert
        posts.AssertEqual(COUNT, TOTAL_COUNT, PAGE_INDEX, PAGE_COUNT);
        Assert.Equal(FOLLOWEE_1_EMAIL_ADDRESS, posts[0].CreatorEmailAddress);
        Assert.Equal(FOLLOWEE_1_NAME, posts[0].CreatorName);
        Assert.Equal(1, posts[0].Id);
        Assert.Equal(FOLLOWEE_2_EMAIL_ADDRESS, posts[1].CreatorEmailAddress);
        Assert.Equal(FOLLOWEE_2_NAME, posts[1].CreatorName);
        Assert.Equal(2, posts[1].Id);
        Assert.Equal(2, posts.Count);
        Assert.Equal(PAGE_COUNT, posts.PageCount);
        Assert.Equal(PAGE_INDEX, posts.PageIndex);
        Assert.Equal(TOTAL_COUNT, posts.TotalCount);
    }
}
