using System;

using Shipstone.OpenBook.Api.Core.Users;

namespace Shipstone.OpenBook.Api.Core.Accounts;

/// <summary>
/// Defines properties to retrieve values claimed by the current user.
/// </summary>
public interface IClaimsService
{
    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    /// <value><c>true</c> if the current user is authenticated; otherwise, <c>false</c>.</value>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the authenticated user.
    /// </summary>
    /// <value>The authenticated user.</value>
    /// <exception cref="InvalidOperationException">The current user is not authenticated.</exception>
    IUser User { get; }
}
