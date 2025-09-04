using System;
using Xunit;

using Shipstone.OpenBook.Api.Core.Followings;
using Shipstone.OpenBook.Api.Core.Posts;

namespace Shipstone.OpenBook.Api.CoreTest;

internal static class Internals
{
    internal static void AssertEqual(
        this IFollowing following,
        String followerEmailAddress,
        String followeeName,
        DateTime followed
    )
    {
        Assert.Equal(followed, following.Followed);
        Assert.Equal(DateTimeKind.Utc, following.Followed.Kind);
        Assert.Equal(followeeName, following.FolloweeName);
        Assert.Equal(followerEmailAddress, following.FollowerEmailAddress);
    }

    internal static void AssertEqual(
        this IPost post,
        long id,
        DateTime created,
        DateTime updated,
        String creatorEmailAddress,
        String creatorUserName,
        String body,
        Nullable<long> parentId = null
    )
    {
        Assert.Equal(body, post.Body);
        Assert.Equal(created, post.Created);
        Assert.Equal(DateTimeKind.Utc, post.Created.Kind);
        Assert.Equal(creatorEmailAddress, post.CreatorEmailAddress);
        Assert.Equal(creatorUserName, post.CreatorName);
        Assert.Equal(id, post.Id);

        if (parentId.HasValue)
        {
            Assert.True(post.ParentId.HasValue);
            Assert.Equal(parentId, post.ParentId.Value);
        }

        else
        {
            Assert.False(post.ParentId.HasValue);
        }

        Assert.Equal(updated, post.Updated);
        Assert.Equal(DateTimeKind.Utc, post.Updated.Kind);
    }

    internal static void SetId(this Object entity, Object id)
    {
        Object?[]? arguments = new Object?[1] { id };

        entity
            .GetType()
            .GetProperty("Id")!
            .GetSetMethod()!
            .Invoke(entity, arguments);
    }
}
