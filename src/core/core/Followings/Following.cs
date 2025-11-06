using System;

namespace Shipstone.OpenBook.Api.Core.Followings;

internal sealed class Following : IFollowing
{
    private readonly DateTime _followed;
    private readonly String _followeeName;
    private readonly String _followerEmailAddress;
    private readonly bool _isSubscribed;

    DateTime IFollowing.Followed => this._followed;
    String IFollowing.FolloweeName => this._followeeName;
    String IFollowing.FollowerEmailAddress => this._followerEmailAddress;
    bool IFollowing.IsSubscribed => this._isSubscribed;

    internal Following(
        String followerEmailAddress,
        String followeeName,
        DateTime followed,
        bool isSubscribed
    )
    {
        this._followed = followed;
        this._followeeName = followeeName;
        this._followerEmailAddress = followerEmailAddress;
        this._isSubscribed = isSubscribed;
    }
}
