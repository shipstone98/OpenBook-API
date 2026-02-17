using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Collections;

namespace Shipstone.OpenBook.Api.Core.Users;

/// <summary>
/// Defines a method to handle listing users.
/// </summary>
public interface IUserListHandler
{
    /// <summary>
    /// Asynchronously lists existing users.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous retrieve operation. The value of <see cref="Task{TResult}.Result" /> contains the listed users.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<IReadOnlyPaginatedList<IUser>> HandleAsync(CancellationToken cancellationToken);
}
