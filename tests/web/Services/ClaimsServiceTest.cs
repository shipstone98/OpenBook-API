using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Web;

using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.WebTest.Services;

public sealed class ClaimsServiceTest
{
    private readonly IClaimsService _claims;

    public ClaimsServiceTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookWebClaims();
        IServiceProvider provider = new MockServiceProvider(services);
        this._claims = provider.GetRequiredService<IClaimsService>();
    }

    [Fact]
    public void TestId_Get()
    {
        // Act and assert
        Assert.Throws<UnauthorizedException>(() => this._claims.Id);
    }
}
