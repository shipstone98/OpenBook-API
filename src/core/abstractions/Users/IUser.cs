using System;
using System.Collections.Generic;

namespace Shipstone.OpenBook.Api.Core.Users;

/// <summary>
/// Represents a user.
/// </summary>
public interface IUser
{
    /// <summary>
    /// Gets the date and time the <see cref="IUser" /> last consented to policy.
    /// </summary>
    /// <value>The date and time the <see cref="IUser" /> last consented to policy.</value>
    DateTime Consented { get; }

    /// <summary>
    /// Gets the date and time the <see cref="IUser" /> was created.
    /// </summary>
    /// <value>The date and time the <see cref="IUser" /> was created.</value>
    DateTime Created { get; }

    /// <summary>
    /// Gets the ID of the <see cref="IUser" />.
    /// </summary>
    /// <value>A <see cref="Guid" /> containing the ID of the <see cref="IUser" />.</value>
    Guid Id { get; }

    /// <summary>
    /// Gets a set containing the roles assigned to the <see cref="IUser" />.
    /// </summary>
    /// <value>A read-only set containing the roles assigned to the <see cref="IUser" />.</value>
    IReadOnlySet<String> Roles { get; }

    /// <summary>
    /// Gets the date and time the <see cref="IUser" /> was last updated.
    /// </summary>
    /// <value>The date and time the <see cref="IUser" /> was last updated.</value>
    DateTime Updated { get; }

    /// <summary>
    /// Gets the name of the <see cref="IUser" />.
    /// </summary>
    /// <value>The name of the <see cref="IUser" />.</value>
    String UserName { get; }
}
