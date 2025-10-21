using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Followings;
using Shipstone.OpenBook.Api.Core.Passwords;
using Shipstone.OpenBook.Api.Core.Posts;
using Shipstone.OpenBook.Api.Core.Users;

using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.CoreTest;

public sealed class CoreServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddOpenBookCore_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                CoreServiceCollectionExtensions.AddOpenBookCore(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddOpenBookCore_Valid()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;

        // Act
        IServiceCollection result =
            CoreServiceCollectionExtensions.AddOpenBookCore(services);

        // Assert
        Assert.Same(services, result);

        IEnumerable<Type> types = new Type[]
        {
            typeof (IAuthenticateHandler),
            typeof (IFollowingCreateHandler),
            typeof (IOtpAuthenticateHandler),
            typeof (IOtpGenerateHandler),
            typeof (IPasswordSetHandler),
            typeof (IPasswordUpdateHandler),
            typeof (IPostAggregateHandler),
            typeof (IPostCreateHandler),
            typeof (IPostDeleteHandler),
            typeof (IPostListHandler),
            typeof (IPostRetrieveHandler),
            typeof (IRefreshAuthenticateHandler),
            typeof (IRegisterHandler),
            typeof (IUserRetrieveHandler)
        };

        foreach (Type type in types)
        {
            ServiceDescriptor descriptor =
                collection.First(s => s.ServiceType.Equals(type));

            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }
    }
}
