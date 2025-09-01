namespace Shipstone.OpenBook.Api.Web.Models;

internal sealed class IdResponse<TId> where TId : struct
{
    private readonly TId _id;

    public TId Id => this._id;

    internal IdResponse(TId id) => this._id = id;
}
