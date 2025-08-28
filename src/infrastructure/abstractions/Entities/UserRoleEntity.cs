using System;

namespace Shipstone.OpenBook.Api.Infrastructure.Entities;

/// <summary>
/// Represents a user-role association.
/// </summary>
public class UserRoleEntity
{
    /// <summary>
    /// Gets or initializes the date and time the associated user was assigned the associated role.
    /// </summary>
    /// <value>The date and time the associated user was assigned the associated role.</value>
    public DateTime Assigned { get; init; }

    /// <summary>
    /// Gets or initializes the ID of the associated role.
    /// </summary>
    /// <value>The ID of the associated role.</value>
    public long RoleId { get; init; }

    /// <summary>
    /// Gets or initializes the ID of the associated user.
    /// </summary>
    /// <value>A <see cref="Guid" /> containing the ID of the associated user.</value>
    public Guid UserId { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRoleEntity" /> class.
    /// </summary>
    public UserRoleEntity() { }
}
