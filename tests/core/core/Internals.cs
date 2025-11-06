using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

using Shipstone.Utilities.Collections;

using Shipstone.OpenBook.Api.Core.Followings;
using Shipstone.OpenBook.Api.Core.Posts;
using Shipstone.OpenBook.Api.Core.Users;

namespace Shipstone.OpenBook.Api.CoreTest;

internal static class Internals
{
    internal static void AssertEmpty<T>(this IReadOnlyPaginatedList<T> list)
    {
        Assert.Empty(list);
        Assert.Equal(1, list.PageCount);
        Assert.Equal(0, list.PageIndex);
        Assert.Equal(0, list.TotalCount);
    }

    internal static void AssertEqual(
        this IFollowing following,
        String followerEmailAddress,
        String followeeName,
        DateTime followed,
        bool isSubscribed
    )
    {
        Assert.Equal(followed, following.Followed);
        Assert.Equal(DateTimeKind.Utc, following.Followed.Kind);
        Assert.Equal(followeeName, following.FolloweeName);
        Assert.Equal(followerEmailAddress, following.FollowerEmailAddress);
        Assert.Equal(isSubscribed, following.IsSubscribed);
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

    internal static void AssertEqual(
        this IUser user,
        Guid id,
        DateTime created,
        DateTime updated,
        String emailAddress,
        String userName,
        String forename,
        String surname,
        DateOnly born,
        DateTime consented,
        IEnumerable<String> roles
    )
    {
        Assert.Equal(born, user.Born);
        Assert.Equal(consented, user.Consented);
        Assert.Equal(DateTimeKind.Utc, user.Consented.Kind);
        Assert.Equal(created, user.Created);
        Assert.Equal(DateTimeKind.Utc, user.Created.Kind);
        Assert.Equal(emailAddress, user.EmailAddress);
        Assert.Equal(forename, user.Forename);
        Assert.Equal(id, user.Id);
        Assert.True(roles.SequenceEqual(user.Roles));
        Assert.Equal(surname, user.Surname);
        Assert.Equal(updated, user.Updated);
        Assert.Equal(userName, user.UserName);
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
