using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a user refresh token repository.
/// </summary>
public interface IUserRefreshTokenRepository
{
    /// <summary>
    /// Asynchronously creates a user refresh token with the specified properties.
    /// </summary>
    /// <param name="userRefreshToken">The user refresh token to create.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous create operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="userRefreshToken" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task CreateAsync(
        UserRefreshTokenEntity userRefreshToken,
        CancellationToken cancellationToken
    );
}
