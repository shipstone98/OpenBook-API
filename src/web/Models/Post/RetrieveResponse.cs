using System;

using Shipstone.OpenBook.Api.Core.Posts;

namespace Shipstone.OpenBook.Api.Web.Models.Post;

internal sealed class RetrieveResponse
{
    private readonly IPost _post;

    String Body => this._post.Body;
    DateTime Created => this._post.Created;
    String Creator => this._post.CreatorName;
    long Id => this._post.Id;
    Nullable<long> ParentId => this._post.ParentId;
    DateTime Updated => this._post.Updated;

    internal RetrieveResponse(IPost post) => this._post = post;
}
