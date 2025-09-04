using System;
using Microsoft.AspNetCore.Builder;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Web.Middleware;

namespace Shipstone.OpenBook.Api.Web;

/// <summary>
/// Provides a set of <c>static</c> (<c>Shared</c> in Visual Basic) methods for adding middleware to objects that implement <see cref="IApplicationBuilder" />.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds OpenBook web claims middleware to the specified <see cref="IApplicationBuilder" />.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder" /> to add middleware to with.</param>
    /// <returns>A reference to <c><paramref name="app" /></c> that can be further used to add middleware.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="app" /></c> is <c>null</c>.</exception>
    public static IApplicationBuilder UseOpenBookWebClaims(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<ClaimsMiddleware>();
    }

    /// <summary>
    /// Adds OpenBook web <see cref="ConflictException" /> middleware to the specified <see cref="IApplicationBuilder" />.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder" /> to add middleware to with.</param>
    /// <returns>A reference to <c><paramref name="app" /></c> that can be further used to add middleware.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="app" /></c> is <c>null</c>.</exception>
    public static IApplicationBuilder UseOpenBookWebConflictExceptionHandling(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<ConflictExceptionHandlingMiddleware>();
    }

    /// <summary>
    /// Adds OpenBook web <see cref="ForbiddenException" /> middleware to the specified <see cref="IApplicationBuilder" />.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder" /> to add middleware to with.</param>
    /// <returns>A reference to <c><paramref name="app" /></c> that can be further used to add middleware.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="app" /></c> is <c>null</c>.</exception>
    public static IApplicationBuilder UseOpenBookWebForbiddenExceptionHandling(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<ForbiddenExceptionHandlingMiddleware>();
    }

    /// <summary>
    /// Adds OpenBook web <see cref="NotFoundException" /> middleware to the specified <see cref="IApplicationBuilder" />.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder" /> to add middleware to with.</param>
    /// <returns>A reference to <c><paramref name="app" /></c> that can be further used to add middleware.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="app" /></c> is <c>null</c>.</exception>
    public static IApplicationBuilder UseOpenBookWebNotFoundExceptionHandling(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<NotFoundExceptionHandlingMiddleware>();
    }
}
