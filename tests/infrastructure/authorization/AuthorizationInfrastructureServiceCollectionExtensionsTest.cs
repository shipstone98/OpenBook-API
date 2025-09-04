using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Infrastructure.Authorization;

using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.AuthorizationTest;

public sealed class AuthorizationInfrastructureServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddOpenBookAuthorization_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                AuthorizationInfrastructureServiceCollectionExtensions.AddOpenBookInfrastructureAuthorization(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddOpenBookAuthorization_Valid()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;

        // Act
        IServiceCollection result =
            AuthorizationInfrastructureServiceCollectionExtensions.AddOpenBookInfrastructureAuthorization(services);

        // Assert
        Assert.True(Object.ReferenceEquals(services, result));

        ServiceDescriptor descriptor =
            services.First(s =>
                s.ServiceType.Equals(typeof (IAuthorizationService)));

        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }
}
