using System;

namespace Shipstone.OpenBook.Api.Web.Models.Post;

internal sealed class CreateRequest
{
#pragma warning disable CS8618
    internal String _body;
    internal Nullable<long> _parentId;
#pragma warning restore CS8618

    public String Body
    {
        set
        {
            this._body = value;
        }
    }

    public Nullable<long> ParentId
    {
        set
        {
            this._parentId = value;
        }
    }
}
