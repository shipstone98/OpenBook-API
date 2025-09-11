using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Collections;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a post repository.
/// </summary>
public interface IPostRepository
{
    /// <summary>
    /// Asynchronously creates a post with the specified properties.
    /// </summary>
    /// <param name="post">The post to create.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous create operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="post" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task CreateAsync(PostEntity post, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously deletes a post with the specified properties.
    /// </summary>
    /// <param name="post">The post to delete.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous delete operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="post" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task DeleteAsync(PostEntity post, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously lists posts for the specified user.
    /// </summary>
    /// <param name="creatorId">A <see cref="Guid" /> containing the ID of the creator of posts to list.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous list operation. The value of <see cref="Task{TResult}.Result" /> contains the listed posts.</returns>
    /// <exception cref="ArgumentException"><c><paramref name="creatorId" /></c> is equal to <see cref="Guid.Empty" />.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<IReadOnlyPaginatedList<PostEntity>> ListForCreatorAsync(
        Guid creatorId,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously retrieves a post with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the post to retrieve.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous retrieve operation. The value of <see cref="Task{TResult}.Result" /> contains the retrieved post, if found; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="id" /></c> is less than or equal to 0 (zero).</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<PostEntity?> RetrieveAsync(
        long id,
        CancellationToken cancellationToken
    );
}
