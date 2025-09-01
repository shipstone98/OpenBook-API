using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Repositories;

public sealed class PostRepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly IPostRepository _repository;

    public PostRepositoryTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookInfrastructureDataEntityFrameworkCore();
        MockDataSource dataSource = new();
        services.AddSingleton<IDataSource>(dataSource);
        IServiceProvider provider = new MockServiceProvider(services);
        this._dataSource = dataSource;
        this._repository = provider.GetRequiredService<IPostRepository>();
    }

    [Fact]
    public async Task TestCreateAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.CreateAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("post", ex.ParamName);
    }

    [Fact]
    public async Task TestCreateAsync_Valid()
    {
        // Arrange
        PostEntity post = new();

        this._dataSource._postsFunc = () =>
        {
            IQueryable<PostEntity> query =
                Array
                    .Empty<PostEntity>()
                    .AsQueryable();

            MockDataSet<PostEntity> dataSet = new(query);
            dataSet._setStateAction = (_, _) => { };
            return dataSet;
        };

        // Act
        await this._repository.CreateAsync(
            post,
            CancellationToken.None
        );

        // Nothing to assert
    }

#region RetrieveAsync method
    [InlineData(Int64.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [Theory]
    public async Task TestRetrieveAsync_Invalid(long id)
    {
        // Act
        ArgumentOutOfRangeException ex =
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                this._repository.RetrieveAsync(id, CancellationToken.None));

        // Assert
        Assert.Equal("id", ex.ParamName);
    }

    [InlineData(1)]
    [InlineData(Int64.MaxValue)]
    [Theory]
    public async Task TestRetrieveAsync_Valid_Contains(long id)
    {
        // Arrange
        this._dataSource._postsFunc = () =>
        {
            IEnumerable<PostEntity> posts = new List<PostEntity>
            {
                new PostEntity
                {
                    Id = id
                }
            };

            IQueryable<PostEntity> query = posts.AsQueryable();
            return new MockDataSet<PostEntity>(query);
        };

        // Act
        PostEntity? post =
            await this._repository.RetrieveAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(post);
        Assert.Equal(id, post.Id);
    }

    [InlineData(1)]
    [InlineData(Int64.MaxValue)]
    [Theory]
    public async Task TestRetrieveAsync_Valid_NotContains(long id)
    {
        // Arrange
        this._dataSource._postsFunc = () =>
        {
            IQueryable<PostEntity> query =
                Array
                    .Empty<PostEntity>().AsQueryable();

            return new MockDataSet<PostEntity>(query);
        };

        // Act
        PostEntity? post =
            await this._repository.RetrieveAsync(id, CancellationToken.None);

        // Assert
        Assert.Null(post);
    }
#endregion
}
