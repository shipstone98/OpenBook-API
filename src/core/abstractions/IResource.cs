using System;

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
}
