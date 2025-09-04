using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Web;

using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.WebTest;

public sealed class WebServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddOpenBookWebClaims_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                WebServiceCollectionExtensions.AddOpenBookWebClaims(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddOpenBookWebClaims_Valid()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;

        // Act
        IServiceCollection result =
            WebServiceCollectionExtensions.AddOpenBookWebClaims(services);

        // Assert
        Assert.True(Object.ReferenceEquals(services, result));

        ServiceDescriptor serviceDescriptor =
            collection.First(s =>
                s.ServiceType.Equals(typeof (IClaimsService)));

        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);

        ServiceDescriptor middlewareDescriptor =
            collection.First(s =>
                s.ServiceType.IsAssignableTo(typeof (IMiddleware)));

        Assert.Equal(ServiceLifetime.Scoped, middlewareDescriptor.Lifetime);
    }

    [Fact]
    public void TestAddOpenBookWebConflictExceptionHandling_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                WebServiceCollectionExtensions.AddOpenBookWebConflictExceptionHandling(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddOpenBookWebConflictExceptionHandling_Valid()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;

        // Act
        IServiceCollection result =
            WebServiceCollectionExtensions.AddOpenBookWebConflictExceptionHandling(services);

        // Assert
        Assert.True(Object.ReferenceEquals(services, result));

        ServiceDescriptor descriptor =
            collection.First(s =>
                s.ServiceType.IsAssignableTo(typeof (IMiddleware)));

        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void TestAddOpenBookWebForbiddenExceptionHandling_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                WebServiceCollectionExtensions.AddOpenBookWebForbiddenExceptionHandling(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddOpenBookWebForbiddenExceptionHandling_Valid()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;

        // Act
        IServiceCollection result =
            WebServiceCollectionExtensions.AddOpenBookWebForbiddenExceptionHandling(services);

        // Assert
        Assert.True(Object.ReferenceEquals(services, result));

        ServiceDescriptor descriptor =
            collection.First(s =>
                s.ServiceType.IsAssignableTo(typeof (IMiddleware)));

        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void TestAddOpenBookWebNotFoundExceptionHandling_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                WebServiceCollectionExtensions.AddOpenBookWebNotFoundExceptionHandling(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddOpenBookWebNotFoundExceptionHandling_Valid()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;

        // Act
        IServiceCollection result =
            WebServiceCollectionExtensions.AddOpenBookWebNotFoundExceptionHandling(services);

        // Assert
        Assert.True(Object.ReferenceEquals(services, result));

        ServiceDescriptor descriptor =
            collection.First(s =>
                s.ServiceType.IsAssignableTo(typeof (IMiddleware)));

        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }
}
