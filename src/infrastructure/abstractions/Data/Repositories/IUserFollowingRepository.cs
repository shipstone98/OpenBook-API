using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a follower-followee association repository.
/// </summary>
public interface IUserFollowingRepository
{
    /// <summary>
    /// Asynchronously creates a follower-followee association with the specified properties.
    /// </summary>
    /// <param name="userFollowing">The follower-followee association to create.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous create operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="userFollowing" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task CreateAsync(
        UserFollowingEntity userFollowing,
        CancellationToken cancellationToken
    );
}
