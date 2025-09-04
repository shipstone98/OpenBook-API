using System;

namespace Shipstone.OpenBook.Api.Infrastructure.Entities;

/// <summary>
/// Represents an association between a follower and a followee.
/// </summary>
public class UserFollowingEntity
{
    /// <summary>
    /// Gets or initializes the date and time the associated follower followed the associated followee.
    /// </summary>
    /// <value>The date and time the associated follower followed the associated followee.</value>
    public DateTime Followed { get; init; }

    /// <summary>
    /// Gets or initializes the ID of the associated followee.
    /// </summary>
    /// <value>A <see cref="Guid" /> containing the ID of the associated followee.</value>
    public Guid FolloweeId { get; init; }

    /// <summary>
    /// Gets or initializes the ID of the associated follower.
    /// </summary>
    /// <value>A <see cref="Guid" /> containing the ID of the associated follower.</value>
    public Guid FollowerId { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserFollowingEntity" /> class.
    /// </summary>
    public UserFollowingEntity() { }
}
