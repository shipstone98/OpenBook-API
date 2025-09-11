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
