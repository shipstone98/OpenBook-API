using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Collections;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Core.Posts;

/// <summary>
/// Defines methods to aggregate posts from multiple followees.
/// </summary>
public interface IPostAggregateHandler
{
    /// <summary>
    /// Asynchronously aggregates posts for the users followed by the current user.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous aggregate operation. The value of <see cref="Task{TResult}.Result" /> contains the aggregated posts.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    /// <exception cref="UnauthorizedException">The current user is not authenticated.</exception>
    Task<IReadOnlyPaginatedList<IPost>> HandleAsync(CancellationToken cancellationToken);
}
