using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest;

public sealed class EntityFrameworkCoreDataInfrastructureServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddOpenBookInfrastructureDataEntityFrameworkCore_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                EntityFrameworkCoreDataInfrastructureServiceCollectionExtensions.AddOpenBookInfrastructureDataEntityFrameworkCore(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddOpenBookInfrastructureDataEntityFrameworkCore_Valid()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;

        // Act
        IServiceCollection result =
            EntityFrameworkCoreDataInfrastructureServiceCollectionExtensions.AddOpenBookInfrastructureDataEntityFrameworkCore(services);

        // Assert
        Assert.Same(services, result);

        IEnumerable<Type> serviceTypes = new Type[]
        {
            typeof (IPostRepository),
            typeof (IRepository),
            typeof (IRoleRepository),
            typeof (IUserDeviceRepository),
            typeof (IUserFollowingRepository),
            typeof (IUserRefreshTokenRepository),
            typeof (IUserRoleRepository),
            typeof (IUserRepository)
        };

        foreach (Type serviceType in serviceTypes)
        {
            ServiceDescriptor descriptor =
                collection.First(s => s.ServiceType.Equals(serviceType));

            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }
    }
}
