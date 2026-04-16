using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities;

using Shipstone.OpenBook.Api.Core.Users;

namespace Shipstone.OpenBook.Api.Core.Accounts;

/// <summary>
/// Defines a method to handle registration.
/// </summary>
public interface IRegisterHandler
{
    /// <summary>
    /// Asynchronously registers a new user with the specified properties.
    /// </summary>
    /// <param name="identityId">A <see cref="Guid" /> containing the identity ID for the new user.</param>
    /// <param name="userName">The name for the new user.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous register operation. The value of <see cref="Task{TResult}.Result" /> contains the new <see cref="IUser" />.</returns>
    /// <exception cref="ArgumentException"><c><paramref name="identityId" /></c> is equal to <see cref="Guid.Empty" /> -or- <c><paramref name="userName" /></c> is not a valid user name.</exception>
    /// <exception cref="ArgumentNullException"><c><paramref name="userName" /></c> is <c>null</c>.</exception>
    /// <exception cref="ConflictException">A user whose identity ID and/or name matches the identity ID of the current user already exists -or- a user whose name matches the provided user name already exists.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<IUser> HandleAsync(
        Guid identityId,
        String userName,
        CancellationToken cancellationToken
    );
}
