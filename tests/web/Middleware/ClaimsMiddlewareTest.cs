using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Web;

using Shipstone.OpenBook.Api.WebTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.WebTest.Middleware;

public sealed class ClaimsMiddlewareTest
{
    private readonly MockHttpContext _context;
    private readonly IClaimsService _claims;
    private readonly RequestDelegate _next;

    public ClaimsMiddlewareTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookWebClaims();
        MockMiddlewareFactory middlewareFactory = new();
        services.AddSingleton<IMiddlewareFactory>(middlewareFactory);
        IServiceProvider provider = new MockServiceProvider(services);
        LinkedList<Func<RequestDelegate, RequestDelegate>> middleware = new();
        MockApplicationBuilder app = new();

        app._useFunc = m =>
        {
            middleware.AddFirst(m);
            return app;
        };

        app.UseOpenBookWebClaims();
        RequestDelegate next = _ => Task.CompletedTask;

        foreach (Func<RequestDelegate, RequestDelegate> item in middleware)
        {
            next = item(next);
        }

        MockHttpContext context = new();
        context._requestServicesFunc = () => provider;

        middlewareFactory._createFunc = t =>
            provider.GetService(t) as IMiddleware;

        middlewareFactory._releaseAction = _ => { };
        this._claims = provider.GetRequiredService<IClaimsService>();
        this._context = context;
        this._next = next;
    }

#region InvokeAsync method
    [Fact]
    public async Task TestInvokeAsync_ClaimsEmpty()
    {
        // Arrange
        this._context._userFunc = () => new();

        // Act
        await this._next.Invoke(this._context);

        // Assert
        Assert.Throws<UnauthorizedException>(() => this._claims.Id);
    }

#region Claims not empty
    [Fact]
    public async Task TestInvokeAsync_ClaimsNotEmpty_ContainsId_ValueNotValid()
    {
        // Arrange
        this._context._userFunc = () =>
        {
            Claim claim = new(ClaimTypes.NameIdentifier, "123456");
            IEnumerable<Claim> claims = new Claim[1] { claim };
            ClaimsIdentity identity = new(claims);
            return new(identity);
        };

        // Act
        await this._next.Invoke(this._context);

        // Assert
        Assert.Throws<UnauthorizedException>(() => this._claims.Id);
    }

    [Fact]
    public async Task TestInvokeAsync_ClaimsNotEmpty_ContainsId_ValueValid()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        this._context._userFunc = () =>
        {
            String idString = id.ToString();
            Claim claim = new(ClaimTypes.NameIdentifier, idString);
            IEnumerable<Claim> claims = new Claim[1] { claim };
            ClaimsIdentity identity = new(claims);
            return new(identity);
        };

        // Act
        await this._next.Invoke(this._context);

        // Assert
        Assert.Equal(id, this._claims.Id);
    }

    [Fact]
    public async Task TestInvokeAsync_ClaimsNotEmpty_NotContainsId()
    {
        // Arrange
        this._context._userFunc = () =>
        {
            Claim claim = new(ClaimTypes.Name, "123456");
            IEnumerable<Claim> claims = new Claim[1] { claim };
            ClaimsIdentity identity = new(claims);
            return new(identity);
        };

        // Act
        await this._next.Invoke(this._context);

        // Assert
        Assert.Throws<UnauthorizedException>(() => this._claims.Id);
    }
#endregion
#endregion
}
