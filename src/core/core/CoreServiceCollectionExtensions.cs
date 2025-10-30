using System;
using Microsoft.Extensions.DependencyInjection;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Followings;
using Shipstone.OpenBook.Api.Core.Passwords;
using Shipstone.OpenBook.Api.Core.Posts;
using Shipstone.OpenBook.Api.Core.Services;
using Shipstone.OpenBook.Api.Core.Users;

namespace Shipstone.OpenBook.Api.Core;

/// <summary>
/// Provides a set of <c>static</c> (<c>Shared</c> in Visual Basic) methods for registering services with objects that implement <see cref="IServiceCollection" />.
/// </summary>
public static class CoreServiceCollectionExtensions
{
    /// <summary>
    /// Registers OpenBook core services with the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to register services with.</param>
    /// <returns>A reference to <c><paramref name="services" /></c> that can be further used to register services.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="services" /></c> is <c>null</c>.</exception>
    public static IServiceCollection AddOpenBookCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddSingleton<IValidationService, ValidationService>()
            .AddScoped<IAuthenticateHandler, AuthenticateHandler>()
            .AddScoped<IAuthenticateService, AuthenticateService>()
            .AddScoped<IFollowingCreateHandler, FollowingCreateHandler>()
            .AddScoped<IFollowingDeleteHandler, FollowingDeleteHandler>()
            .AddScoped<IOtpAuthenticateHandler, OtpAuthenticateHandler>()
            .AddScoped<IOtpService, OtpService>()
            .AddScoped<IPasswordResetHandler, PasswordResetHandler>()
            .AddScoped<IPasswordSetHandler, PasswordSetHandler>()
            .AddScoped<IPasswordUpdateHandler, PasswordUpdateHandler>()
            .AddScoped<IPostAggregateHandler, PostAggregateHandler>()
            .AddScoped<IPostCreateHandler, PostCreateHandler>()
            .AddScoped<IPostDeleteHandler, PostDeleteHandler>()
            .AddScoped<IPostListHandler, PostListHandler>()
            .AddScoped<IPostRetrieveHandler, PostRetrieveHandler>()
            .AddScoped<IRefreshAuthenticateHandler, RefreshAuthenticateHandler>()
            .AddScoped<IRegisterHandler, RegisterHandler>()
            .AddScoped<IUnregisterHandler, UnregisterHandler>()
            .AddScoped<IUserRetrieveHandler, UserRetrieveHandler>()
            .AddScoped<IUserUpdateHandler, UserUpdateHandler>();
    }
}
