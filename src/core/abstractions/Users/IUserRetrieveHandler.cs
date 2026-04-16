using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities;

namespace Shipstone.OpenBook.Api.Core.Users;

/// <summary>
/// Defines a method to handle retrieving a user.
/// </summary>
public interface IUserRetrieveHandler
{
    /// <summary>
    /// Asynchronously retrieves the user for the specified identity ID.
    /// </summary>
    /// <param name="identityId">A <see cref="Guid" /> containing the identity ID of the user.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous retrieve operation. The value of <see cref="Task{TResult}.Result" /> contains the retrieved <see cref="IUser" />.</returns>
    /// <exception cref="ArgumentException"><c><paramref name="identityId" /></c> is equal to <see cref="Guid.Empty" />.</exception>
    /// <exception cref="NotFoundException">A user whose identity ID matches the provided identity ID could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    /// <exception cref="UserNotActiveException">The user whose identity ID matches the provided identity ID is not active.</exception>
    Task<IUser> HandleAsync(
        Guid identityId,
        CancellationToken cancellationToken
    );
}
