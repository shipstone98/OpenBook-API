using System;

namespace Shipstone.OpenBook.Api.Core.Followings;

/// <summary>
/// Represents an assocation between a follower and a followee.
/// </summary>
public interface IFollowing
{
    /// <summary>
    /// Gets or initializes the date and time the associated follower followed the associated followee.
    /// </summary>
    /// <value>The date and time the associated follower followed the associated followee.</value>
    DateTime Followed { get; }

    /// <summary>
    /// Gets the name of the associated followee.
    /// </summary>
    /// <value>The name of the associated followee.</value>
    String FolloweeName { get; }

    /// <summary>
    /// Gets the email address of the associated follower.
    /// </summary>
    /// <value>The email address of the associated follower.</value>
    String FollowerEmailAddress { get; }
}
