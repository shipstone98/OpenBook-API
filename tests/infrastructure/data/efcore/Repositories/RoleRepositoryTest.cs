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

public sealed class RoleRepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly IRoleRepository _repository;

    public RoleRepositoryTest()
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
        this._repository = provider.GetRequiredService<IRoleRepository>();
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
        this._dataSource._rolesFunc = () =>
        {
            IEnumerable<RoleEntity> roles = new List<RoleEntity>
            {
                new RoleEntity
                {
                    Id = id
                }
            };

            IQueryable<RoleEntity> query = roles.AsQueryable();
            return new MockDataSet<RoleEntity>(query);
        };

        // Act
        RoleEntity? role =
            await this._repository.RetrieveAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(role);
        Assert.Equal(id, role.Id);
    }

    [InlineData(1)]
    [InlineData(Int64.MaxValue)]
    [Theory]
    public async Task TestRetrieveAsync_Valid_NotContains(long id)
    {
        // Arrange
        this._dataSource._rolesFunc = () =>
        {
            IQueryable<RoleEntity> query =
                Array
                    .Empty<RoleEntity>().AsQueryable();

            return new MockDataSet<RoleEntity>(query);
        };

        // Act
        RoleEntity? role =
            await this._repository.RetrieveAsync(id, CancellationToken.None);

        // Assert
        Assert.Null(role);
    }
#endregion
}
