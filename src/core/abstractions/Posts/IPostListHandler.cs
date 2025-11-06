using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Collections;

namespace Shipstone.OpenBook.Api.Core.Posts;

/// <summary>
/// Defines methods to list posts.
/// </summary>
public interface IPostListHandler
{
    /// <summary>
    /// Asynchronously lists posts with the specified parent ID.
    /// </summary>
    /// <param name="parentId">The parent ID of the posts to list.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous list operation. The value of <see cref="Task{TResult}.Result" /> contains the listed posts.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="parentId" /></c> is less than or equal to 0 (zero).</exception>
    /// <exception cref="NotFoundException">A post whose ID matches the provided parent ID could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<IReadOnlyPaginatedList<IPost>> HandleAsync(
        long parentId,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously lists posts for the specified user.
    /// </summary>
    /// <param name="userName">The name of the creator of the posts to list.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous list operation. The value of <see cref="Task{TResult}.Result" /> contains the listed posts.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="userName" /></c> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">A user whose name matches the provided user name could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<IReadOnlyPaginatedList<IPost>> HandleAsync(
        String userName,
        CancellationToken cancellationToken
    );
}
