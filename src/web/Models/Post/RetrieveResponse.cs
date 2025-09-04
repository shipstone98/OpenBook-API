using System;

using Shipstone.OpenBook.Api.Core.Posts;

namespace Shipstone.OpenBook.Api.Web.Models.Post;

internal sealed class RetrieveResponse
{
    private readonly IPost _post;

    public String Body => this._post.Body;
    public DateTime Created => this._post.Created;
    public String Creator => this._post.CreatorName;
    public long Id => this._post.Id;
    public Nullable<long> ParentId => this._post.ParentId;
    public DateTime Updated => this._post.Updated;

    internal RetrieveResponse(IPost post) => this._post = post;
}
