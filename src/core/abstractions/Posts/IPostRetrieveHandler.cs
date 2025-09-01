using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.OpenBook.Api.Core.Posts;

/// <summary>
/// Defines a method to retrieve a post.
/// </summary>
public interface IPostRetrieveHandler
{
    /// <summary>
    /// Asynchronously retrieves a post with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the post to retrieve.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous retrieve operation. The value of <see cref="Task{TResult}.Result" /> contains the retrieved <see cref="IPost" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="id" /></c> is less than or equal to 0 (zero).</exception>
    /// <exception cref="NotFoundException">A post whose ID matches the provided ID could not be found -or- a user whose ID matches the creator ID of the post whose ID matches the provided ID could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<IPost> HandleAsync(long id, CancellationToken cancellationToken);
}
