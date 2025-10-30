using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a user decice repository.
/// </summary>
public interface IUserDeviceRepository
{
    /// <summary>
    /// Asynchronously deletes a user device with the specified properties.
    /// </summary>
    /// <param name="userDevice">The user device to delete.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous delete operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="userDevice" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task DeleteAsync(
        UserDeviceEntity userDevice,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously lists all user devices with the specified user ID.
    /// </summary>
    /// <param name="userId">A <see cref="Guid" /> containing the ID of the user to list associated devices for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous list operation. The value of <see cref="Task{TResult}.Result" /> contains the listed user devices.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="userId" /></c> is equal to <see cref="Guid.Empty" />.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<UserDeviceEntity[]> ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken
    );
}
