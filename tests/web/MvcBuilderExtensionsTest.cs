using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Web;

using Shipstone.OpenBook.Api.WebTest.Mocks;

namespace Shipstone.OpenBook.Api.WebTest;

public sealed class MvcBuilderExtensionsTest
{
    [Fact]
    public void TestAddOpenBookControllers_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                MvcBuilderExtensions.AddOpenBookControllers(null!));

        // Assert
        Assert.Equal("builder", ex.ParamName);
    }

    [Fact]
    public void TestAddOpenBookControllers_Valid()
    {
        // Arrange
        MockApplicationPartManager manager = new();
        MockMvcBuilder builder = new();
        builder._partManagerFunc = () => manager;

        // Act
        IMvcBuilder result =
            MvcBuilderExtensions.AddOpenBookControllers(builder);

        // Assert
        Assert.Same(builder, result);
        Assert.NotEmpty(manager.FeatureProviders);
    }
}
