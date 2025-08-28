using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a role repository.
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Asynchronously retrieves a role with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the role to retrieve.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous retrieve operation. The value of <see cref="Task{TResult}.Result" /> contains the retrieved role, if found; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="id" /></c> is less than or equal to 0 (zero).</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<RoleEntity?> RetrieveAsync(
        long id,
        CancellationToken cancellationToken
    );
}
