using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.OpenBook.Api.Infrastructure.Mail;

/// <summary>
/// Defines methods to send electronic mail.
/// </summary>
public interface IMailService
{
    /// <summary>
    /// Asynchronously sends registration mail to the current user.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous send operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SendRegistrationAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously sends unregistration mail to the current user.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous send operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SendUnregistrationAsync(CancellationToken cancellationToken);
}
