using System;

namespace Shipstone.OpenBook.Api.Core.Followings;

internal sealed class Following : IFollowing
{
    private readonly DateTime _followed;
    private readonly String _followeeName;
    private readonly String _followerName;
    private readonly bool _isSubscribed;

    DateTime IFollowing.Followed => this._followed;
    String IFollowing.FolloweeName => this._followeeName;
    String IFollowing.FollowerName => this._followerName;
    bool IFollowing.IsSubscribed => this._isSubscribed;

    internal Following(
        String followerName,
        String followeeName,
        DateTime followed,
        bool isSubscribed
    )
    {
        this._followed = followed;
        this._followeeName = followeeName;
        this._followerName = followerName;
        this._isSubscribed = isSubscribed;
    }
}
