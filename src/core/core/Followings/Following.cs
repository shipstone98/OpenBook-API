using System;

namespace Shipstone.OpenBook.Api.Core.Followings;

internal sealed class Following : IFollowing
{
    private readonly DateTime _followed;
    private readonly String _followeeName;
    private readonly String _followerEmailAddress;

    DateTime IFollowing.Followed => this._followed;
    String IFollowing.FolloweeName => this._followeeName;
    String IFollowing.FollowerEmailAddress => this._followerEmailAddress;

    internal Following(
        String followerEmailAddress,
        String followeeName,
        DateTime followed
    )
    {
        this._followed = followed;
        this._followeeName = followeeName;
        this._followerEmailAddress = followerEmailAddress;
    }
}
