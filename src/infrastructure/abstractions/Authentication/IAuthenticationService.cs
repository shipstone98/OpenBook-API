using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Authentication;

/// <summary>
/// Defines methods to handle authentication.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Asynchronously generates a one-time passcode (OTP) for the specified user.
    /// </summary>
    /// <param name="user">The user to generate a one-time passcode for.</param>
    /// <param name="now">The current date and time.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous generate operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="user" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task GenerateOtpAsync(
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    );
}
