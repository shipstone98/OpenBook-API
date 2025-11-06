using System;

using Shipstone.OpenBook.Api.Core.Followings;

namespace Shipstone.OpenBook.Api.Web.Models.Following;

internal sealed class RetrieveResponse
{
    private readonly IFollowing _following;

    public DateTime Followed => this._following.Followed;
    public bool IsSubscribed => this._following.IsSubscribed;

    internal RetrieveResponse(IFollowing following) =>
        this._following = following;
}
