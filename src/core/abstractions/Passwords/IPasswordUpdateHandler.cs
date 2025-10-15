using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Identity;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Users;

namespace Shipstone.OpenBook.Api.Core.Passwords;

/// <summary>
/// Defines a method to handle password updating.
/// </summary>
public interface IPasswordUpdateHandler
{
    /// <summary>
    /// Asynchronously updates the password of the current user.
    /// </summary>
    /// <param name="passwordCurrent">The current password of the current user.</param>
    /// <param name="passwordNew">The new password for the current user.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous update operation. The value of <see cref="Task{TResult}.Result" /> contains the updated <see cref="IUser" />.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="passwordCurrent" /></c> is <c>null</c> -or- <c><paramref name="passwordNew" /></c> is <c>null</c>.</exception>
    /// <exception cref="ForbiddenException">The current user has not verified their email address.</exception>
    /// <exception cref="NotFoundException">The current user could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    /// <exception cref="PasswordNotValidException"><c><paramref name="passwordNew" /></c> is not a valid password.</exception>
    /// <exception cref="UserNotActiveException">The current user is not active.</exception>
    /// <exception cref="UnauthorizedException">The current user is not authenticated.</exception>
    Task<IUser> HandleAsync(
        String passwordCurrent,
        String passwordNew,
        CancellationToken cancellationToken
    );
}
