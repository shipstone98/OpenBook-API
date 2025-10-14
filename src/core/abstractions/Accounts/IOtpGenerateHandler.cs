using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.OpenBook.Api.Core.Accounts;

/// <summary>
/// Defines a method to handle one-time passcode (OTP) generation.
/// </summary>
public interface IOtpGenerateHandler
{
    /// <summary>
    /// Asynchronously generates a one-time passcode (OTP) for the specified user.
    /// </summary>
    /// <param name="emailAddress">The email address of the user to generate a one-time passcode (OTP) for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous handle operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="emailAddress" /></c> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">A user whose email address matches the provided email address could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    /// <exception cref="UserNotActiveException">The user whose email address matches the provided email address is not active.</exception>
    Task HandleAsync(
        String emailAddress,
        CancellationToken cancellationToken
    );
}
