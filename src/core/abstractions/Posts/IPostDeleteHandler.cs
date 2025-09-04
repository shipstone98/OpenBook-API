using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Core.Posts;

/// <summary>
/// Defines a method to delete a post.
/// </summary>
public interface IPostDeleteHandler
{
    /// <summary>
    /// Asynchronously deletes a post with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the post to delete.</param>
    /// <param name="policy">The authorization policy to use when authorizing the current user.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous delete operation. The value of <see cref="Task{TResult}.Result" /> contains the deleted <see cref="IPost" />.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="policy" /></c> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="id" /></c> is less than or equal to 0 (zero).</exception>
    /// <exception cref="ForbiddenException">The current user is not authorized to delete the post whose ID matches the provided ID.</exception>
    /// <exception cref="NotFoundException">A post whose ID matches the provided ID could not be found -or- a user whose ID matches the creator ID of the post whose ID matches the provided ID could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<IPost> HandleAsync(
        long id,
        String policy,
        CancellationToken cancellationToken
    );
}
