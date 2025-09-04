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

using Shipstone.OpenBook.Api.CoreTest.Mocks;
using Shipstone.OpenBook.Api.Test.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest.Posts;

public sealed class PostCreateHandlerTest
{
    private readonly MockClaimsService _claims;
    private readonly IPostCreateHandler _handler;
    private readonly MockRepository _repository;

    public PostCreateHandlerTest()
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
        this._handler = provider.GetRequiredService<IPostCreateHandler>();
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
        Assert.Equal("builder", ex.ParamName);
    }

#region Valid arguments
    [Fact]
    public async Task TestHandleAsync_Valid_Failure_ParentIdNotNull()
    {
        // Arrange
        this._claims._idFunc = Guid.NewGuid;

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();
            posts._createAction = _ => { };
            return posts;
        };

        this._repository._saveAction = () => throw new();

        // Act and assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
                new PostBuilder
                {
                    ParentId = 12345
                },
                CancellationToken.None
            ));
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Failure_ParentIdNull()
    {
        // Arrange
        Exception innerException = new();
        this._claims._idFunc = Guid.NewGuid;

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();
            posts._createAction = _ => { };
            return posts;
        };

        this._repository._saveAction = () => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<Exception>(() =>
                this._handler.HandleAsync(
                    new PostBuilder { },
                    CancellationToken.None
                ));

        // Assert
        Assert.True(Object.ReferenceEquals(innerException, ex));
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Success()
    {
        // Arrange
        const long ID = 12345;
        const String CREATOR_EMAIL_ADDRESS = "john.doe@lampada.co";
        const String CREATOR_USER_NAME = "johndoe2025";
        const long PARENT_ID = 67890;
        const String BODY = "My post body.";
        this._claims._idFunc = Guid.NewGuid;

        this._repository._postsFunc = () =>
        {
            MockPostRepository posts = new();
            posts._createAction = p => p.SetId(ID);
            return posts;
        };

        this._repository._saveAction = () => { };
        this._claims._emailAddressFunc = () => CREATOR_EMAIL_ADDRESS;
        this._claims._userNameFunc = () => CREATOR_USER_NAME;
        DateTime notBefore = DateTime.UtcNow;

        // Act
        IPost post =
            await this._handler.HandleAsync(
                new PostBuilder
                {
                    Body = BODY,
                    ParentId = PARENT_ID
                },
                CancellationToken.None
            );

        // Assert
        Assert.False(DateTime.Compare(notBefore, post.Created) > 0);

        post.AssertEqual(
            ID,
            post.Created,
            post.Created,
            CREATOR_EMAIL_ADDRESS,
            CREATOR_USER_NAME,
            BODY,
            PARENT_ID
        );
    }
#endregion
#endregion
}
