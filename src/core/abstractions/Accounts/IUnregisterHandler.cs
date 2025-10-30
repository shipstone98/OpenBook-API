using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.OpenBook.Api.Core.Accounts;

/// <summary>
/// Defines a method to handle unregistration.
/// </summary>
public interface IUnregisterHandler
{
    /// <summary>
    /// Asynchronously deletes the current user.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous unregister operation.</returns>
    /// <exception cref="NotFoundException">The current user could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    /// <exception cref="UnauthorizedException">The current user is not authenticated.</exception>
    /// <exception cref="UserNotActiveException">The current user is not active.</exception>
    Task HandleAsync(CancellationToken cancellationToken);
}
