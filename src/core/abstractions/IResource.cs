using System;
using System.Collections.Generic;

namespace Shipstone.OpenBook.Api.Core;

/// <summary>
/// Represents a resource created by a user.
/// </summary>
public interface IResource
{
    /// <summary>
    /// Gets the ID of the user that created the <see cref="IResource" />.
    /// </summary>
    /// <value>A <see cref="Guid" /> containing the ID of the user that created the <see cref="IResource" />.</value>
    Guid CreatorId { get; }

    /// <summary>
    /// Gets a set containing the roles assigned to the user that created the <see cref="IResource" />.
    /// </summary>
    /// <value>A read-only set containing the roles assigned to the user that created the <see cref="IResource" />.</value>
    IReadOnlySet<String> CreatorRoles { get; }
}
