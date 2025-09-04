using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;

/// <summary>
/// Represents a data source.
/// </summary>
public interface IDataSource
{
    /// <summary>
    /// Gets the post data set.
    /// </summary>
    /// <value>An <see cref="IDataSet{TEntity}" /> containing the posts.</value>
    IDataSet<PostEntity> Posts { get; }

    /// <summary>
    /// Gets the role data set.
    /// </summary>
    /// <value>An <see cref="IDataSet{TEntity}" /> containing the roles.</value>
    IDataSet<RoleEntity> Roles { get; }

    /// <summary>
    /// Gets the follower-followee association data set.
    /// </summary>
    /// <value>An <see cref="IDataSet{TEntity}" /> containing the follower-followee associations.</value>
    IDataSet<UserFollowingEntity> UserFollowings { get; }

    /// <summary>
    /// Gets the user refresh token data set.
    /// </summary>
    /// <value>An <see cref="IDataSet{TEntity}" /> containing the user refresh tokens.</value>
    IDataSet<UserRefreshTokenEntity> UserRefreshTokens { get; }

    /// <summary>
    /// Gets the user-role association data set.
    /// </summary>
    /// <value>An <see cref="IDataSet{TEntity}" /> containing the user-role associations.</value>
    IDataSet<UserRoleEntity> UserRoles { get; }

    /// <summary>
    /// Gets the user data set.
    /// </summary>
    /// <value>An <see cref="IDataSet{TEntity}" /> containing the users.</value>
    IDataSet<UserEntity> Users { get; }

    /// <summary>
    /// Asynchronously saves all changes to the data source.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous save operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SaveAsync(CancellationToken cancellationToken);
}
