using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a repository.
/// </summary>
public interface IRepository
{
    /// <summary>
    /// Gets the post repository.
    /// </summary>
    /// <value>The post repository.</value>
    IPostRepository Posts { get; }

    /// <summary>
    /// Gets the role repository.
    /// </summary>
    /// <value>The role repository.</value>
    IRoleRepository Roles { get; }

    /// <summary>
    /// Gets the follower-followee association repository.
    /// </summary>
    /// <value>The follower-followee association repository.</value>
    IUserFollowingRepository UserFollowings { get; }

    /// <summary>
    /// Gets the user refresh token repository.
    /// </summary>
    /// <value>The user refresh token repository.</value>
    IUserRefreshTokenRepository UserRefreshTokens { get; }

    /// <summary>
    /// Gets the user-role association repository.
    /// </summary>
    /// <value>The user-role association repository.</value>
    IUserRoleRepository UserRoles { get; }

    /// <summary>
    /// Gets the user repository.
    /// </summary>
    /// <value>The user repository.</value>
    IUserRepository Users { get; }

    /// <summary>
    /// Asynchronously saves all changes to the data source.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous save operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SaveAsync(CancellationToken cancellationToken);
}
