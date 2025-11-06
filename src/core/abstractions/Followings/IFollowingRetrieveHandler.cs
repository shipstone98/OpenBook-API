using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Core.Followings;

/// <summary>
/// Defines a method to retrieve following information on a user.
/// </summary>
public interface IFollowingRetrieveHandler
{
    /// <summary>
    /// Asynchronously retrieves following information on the specified user for the current user.
    /// </summary>
    /// <param name="userName">The name of the user to retrieve following information on.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous retrieve operation. The value of <see cref="Task{TResult}.Result" /> contains the retrieved <see cref="IFollowing" />.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="userName" /></c> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">A user whose name matches the provided user name could not be found -or- the current user is not following the user whose name matches the provided user name.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    /// <exception cref="UserNotActiveException">The user whose name matches the provided user name is not active.</exception>
    Task<IFollowing> HandleAsync(
        String userName,
        CancellationToken cancellationToken
    );
}
