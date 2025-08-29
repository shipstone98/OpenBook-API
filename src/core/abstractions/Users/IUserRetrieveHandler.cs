using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Core.Users;

/// <summary>
/// Defines a method to handle retrieving a user.
/// </summary>
public interface IUserRetrieveHandler
{
    /// <summary>
    /// Asynchronously retrieves the current user.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous retrieve operation. The value of <see cref="Task{TResult}.Result" /> contains the retrieved <see cref="IUser" />.</returns>
    /// <exception cref="NotFoundException">The current user could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    /// <exception cref="UnauthorizedException">The current user is not authenticated.</exception>
    /// <exception cref="UserNotActiveException">The current user is not active.</exception>
    Task<IUser> HandleAsync(CancellationToken cancellationToken);
}
