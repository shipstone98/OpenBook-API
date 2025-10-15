using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Infrastructure.Notifications;

using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.NotificationsTest;

public sealed class NotificationsInfrastructureServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddOpenBookNotifications_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                NotificationsInfrastructureServiceCollectionExtensions.AddOpenBookInfrastructureNotifications(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddOpenBookNotifications_Valid()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;

        // Act
        IServiceCollection result =
            NotificationsInfrastructureServiceCollectionExtensions.AddOpenBookInfrastructureNotifications(services);

        // Assert
        Assert.Same(services, result);

        ServiceDescriptor descriptor =
            services.First(s =>
                s.ServiceType.Equals(typeof (INotificationService)));

        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }
}
