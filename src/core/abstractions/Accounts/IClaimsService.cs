using System;

namespace Shipstone.OpenBook.Api.Core.Accounts;

/// <summary>
/// Defines properties to retrieve values claimed by the current user.
/// </summary>
public interface IClaimsService
{
    /// <summary>
    /// Gets the email address of the current user.
    /// </summary>
    /// <value>The email address of the current user.</value>
    /// <exception cref="UnauthorizedException">The current user is not authenticated.</exception>
    String EmailAddress { get; }

    /// <summary>
    /// Gets the ID of the current user.
    /// </summary>
    /// <value>A <see cref="Guid" /> containing the ID of the current user.</value>
    /// <exception cref="UnauthorizedException">The current user is not authenticated.</exception>
    Guid Id { get; }

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    /// <value><c>true</c> if the current user is authenticated; otherwise, <c>false</c>.</value>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the name of the current user.
    /// </summary>
    /// <value>The name of the current user.</value>
    /// <exception cref="UnauthorizedException">The current user is not authenticated.</exception>
    String UserName { get; }
}
