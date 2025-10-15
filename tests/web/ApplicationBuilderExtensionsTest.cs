using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Xunit;

using Shipstone.OpenBook.Api.WebTest.Mocks;

namespace Shipstone.OpenBook.Api.WebTest;

public sealed class ApplicationBuilderExtensionsTest
{
    [Fact]
    public void TestUseOpenBookWebClaims_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                Web.ApplicationBuilderExtensions.UseOpenBookWebClaims(null!));

        // Assert
        Assert.Equal("app", ex.ParamName);
    }

    [Fact]
    public void TestUseOpenBookWebClaims_Valid()
    {
        // Arrange
        ICollection<Func<RequestDelegate, RequestDelegate>> middleware =
            new List<Func<RequestDelegate, RequestDelegate>>();

        MockApplicationBuilder app = new();

        app._useFunc = m =>
        {
            middleware.Add(m);
            return app;
        };

        // Act
        IApplicationBuilder result =
            Web.ApplicationBuilderExtensions.UseOpenBookWebClaims(app);

        // Assert
        Assert.Same(app, result);
        Assert.NotEmpty(middleware);
    }

    [Fact]
    public void TestUseOpenBookWebConflictExceptionHandling_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                Web.ApplicationBuilderExtensions.UseOpenBookWebConflictExceptionHandling(null!));

        // Assert
        Assert.Equal("app", ex.ParamName);
    }

    [Fact]
    public void TestUseOpenBookWebConflictExceptionHandling_Valid()
    {
        // Arrange
        ICollection<Func<RequestDelegate, RequestDelegate>> middleware =
            new List<Func<RequestDelegate, RequestDelegate>>();

        MockApplicationBuilder app = new();

        app._useFunc = m =>
        {
            middleware.Add(m);
            return app;
        };

        // Act
        IApplicationBuilder result =
            Web.ApplicationBuilderExtensions.UseOpenBookWebConflictExceptionHandling(app);

        // Assert
        Assert.Same(app, result);
        Assert.NotEmpty(middleware);
    }

    [Fact]
    public void TestUseOpenBookWebForbiddenExceptionHandling_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                Web.ApplicationBuilderExtensions.UseOpenBookWebForbiddenExceptionHandling(null!));

        // Assert
        Assert.Equal("app", ex.ParamName);
    }

    [Fact]
    public void TestUseOpenBookWebForbiddenExceptionHandling_Valid()
    {
        // Arrange
        ICollection<Func<RequestDelegate, RequestDelegate>> middleware =
            new List<Func<RequestDelegate, RequestDelegate>>();

        MockApplicationBuilder app = new();

        app._useFunc = m =>
        {
            middleware.Add(m);
            return app;
        };

        // Act
        IApplicationBuilder result =
            Web.ApplicationBuilderExtensions.UseOpenBookWebForbiddenExceptionHandling(app);

        // Assert
        Assert.Same(app, result);
        Assert.NotEmpty(middleware);
    }

    [Fact]
    public void TestUseOpenBookWebNotFoundExceptionHandling_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                Web.ApplicationBuilderExtensions.UseOpenBookWebNotFoundExceptionHandling(null!));

        // Assert
        Assert.Equal("app", ex.ParamName);
    }

    [Fact]
    public void TestUseOpenBookWebNotFoundExceptionHandling_Valid()
    {
        // Arrange
        ICollection<Func<RequestDelegate, RequestDelegate>> middleware =
            new List<Func<RequestDelegate, RequestDelegate>>();

        MockApplicationBuilder app = new();

        app._useFunc = m =>
        {
            middleware.Add(m);
            return app;
        };

        // Act
        IApplicationBuilder result =
            Web.ApplicationBuilderExtensions.UseOpenBookWebNotFoundExceptionHandling(app);

        // Assert
        Assert.Same(app, result);
        Assert.NotEmpty(middleware);
    }
}
