using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a user-role association repository.
/// </summary>
public interface IUserRoleRepository
{
    /// <summary>
    /// Asynchronously creates a user-role assocation with the specified properties.
    /// </summary>
    /// <param name="userRole">The user-role association to create.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous create operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="userRole" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task CreateAsync(
        UserRoleEntity userRole,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously lists all user-role associations with the specified user ID.
    /// </summary>
    /// <param name="userId">A <see cref="Guid" /> containing the ID of the user to list associated roles for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous list operation. The value of <see cref="Task{TResult}.Result" /> contains the listed user-role associations.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="userId" /></c> is equal to <see cref="Guid.Empty" />.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<UserRoleEntity[]> ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken
    );
}
