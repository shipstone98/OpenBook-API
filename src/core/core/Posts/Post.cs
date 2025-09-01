using System;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Posts;

internal sealed class Post : IPost
{
    private readonly String _creatorEmailAddress;
    private readonly String _creatorName;
    private readonly PostEntity _post;

    String IPost.Body => this._post.Body;
    DateTime IPost.Created => this._post.Created;
    String IPost.CreatorEmailAddress => this._creatorEmailAddress;
    String IPost.CreatorName => this._creatorName;
    long IPost.Id => this._post.Id;
    Nullable<long> IPost.ParentId => this._post.ParentId;
    DateTime IPost.Updated => this._post.Updated;

    internal Post(
        PostEntity post,
        String creatorEmailAddress,
        String creatorName
    )
    {
        this._creatorEmailAddress = creatorEmailAddress;
        this._creatorName = creatorName;
        this._post = post;
    }
}
