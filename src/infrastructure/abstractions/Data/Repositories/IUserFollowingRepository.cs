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

    /// <summary>
    /// Asynchronously deletes a follower-followee association with the specified properties.
    /// </summary>
    /// <param name="userFollowing">The follower-followee association to delete.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous delete operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="userFollowing" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task DeleteAsync(
        UserFollowingEntity userFollowing,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously lists all follower-followee associations with the specified followee ID.
    /// </summary>
    /// <param name="followeeId">A <see cref="Guid" /> containing the ID of the followee to list associated followers for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous list operation. The value of <see cref="Task{TResult}.Result" /> contains the listed follower-followee associations.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="followeeId" /></c> is equal to <see cref="Guid.Empty" />.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<UserFollowingEntity[]> ListForFolloweeAsync(
        Guid followeeId,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously lists all follower-followee associations with the specified follower ID.
    /// </summary>
    /// <param name="followerId">A <see cref="Guid" /> containing the ID of the follower to list associated followees for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous list operation. The value of <see cref="Task{TResult}.Result" /> contains the listed follower-followee associations.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="followerId" /></c> is equal to <see cref="Guid.Empty" />.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<UserFollowingEntity[]> ListForFollowerAsync(
        Guid followerId,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously retrieves a follower-followee association with the specified follower and followee IDs.
    /// </summary>
    /// <param name="followerId">A <see cref="Guid" /> containing the ID of the follower to retrieve the association for.</param>
    /// <param name="followeeId">A <see cref="Guid" /> containing the ID of the followee to retrieve the association for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous retrieve operation. The value of <see cref="Task{TResult}.Result" /> contains the retrieved follower-followee associations, if found; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="followerId" /></c> is equal to <see cref="Guid.Empty" /> -or- <c><paramref name="followeeId" /></c> is equal to <see cref="Guid.Empty" />.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<UserFollowingEntity?> RetrieveAsync(
        Guid followerId,
        Guid followeeId,
        CancellationToken cancellationToken
    );
}
