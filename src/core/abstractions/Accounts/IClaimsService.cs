using System;

namespace Shipstone.OpenBook.Api.Core.Accounts;

/// <summary>
/// Defines properties to retrieve values claimed by the current user.
/// </summary>
public interface IClaimsService
{
    /// <summary>
    /// Gets the ID of the current user.
    /// </summary>
    /// <value>A <see cref="Guid" /> containing the ID of the current user.</value>
    /// <exception cref="UnauthorizedException">The current user is not authenticated.</exception>
    Guid Id { get; }
}
