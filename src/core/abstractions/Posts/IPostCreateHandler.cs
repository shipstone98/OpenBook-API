using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.OpenBook.Api.Core.Posts;

/// <summary>
/// Defines a method to create a post.
/// </summary>
public interface IPostCreateHandler
{
    /// <summary>
    /// Asynchronously creates a new post.
    /// </summary>
    /// <param name="builder">The <see cref="PostBuilder" /> that represents the new post.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous retrieve operation. The value of <see cref="Task{TResult}.Result" /> contains the created <see cref="IPost" />.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="builder" /></c> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">A post whose ID matches the provided parent ID could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<IPost> HandleAsync(
        PostBuilder builder,
        CancellationToken cancellationToken
    );
}
