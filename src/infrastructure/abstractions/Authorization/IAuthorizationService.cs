using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;

namespace Shipstone.OpenBook.Api.Infrastructure.Authorization;

/// <summary>
/// Defines a method to authorize the current user.
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Asynchronously authorizes the current user to modify the specified resource.
    /// </summary>
    /// <param name="resource">The resource to authorize modification for.</param>
    /// <param name="policy">The name of the policy to authorize against.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous authorize operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="resource" /></c> is <c>null</c> -or- <c><paramref name="policy" /></c> is <c>null</c>.</exception>
    /// <exception cref="ForbiddenException">The current user is not authorized to modify the provided resource.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task AuthorizeAsync(
        IResource resource,
        String policy,
        CancellationToken cancellationToken
    );
}
