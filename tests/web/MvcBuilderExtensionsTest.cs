using System;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Web;

using Shipstone.Test.Mocks;

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
        ApplicationPartManager manager = new();
        MockMvcBuilder builder = new();
        builder._partManagerFunc = () => manager;

        // Act
        IMvcBuilder result =
            MvcBuilderExtensions.AddOpenBookControllers(builder);

        // Assert
        Assert.Same(builder, result);
        
        Assert.Contains(
            manager.FeatureProviders,
            fp => fp is ControllerFeatureProvider
        );
    }
}
