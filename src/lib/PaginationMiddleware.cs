using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

using Shipstone.Extensions.Pagination;

namespace Shipstone.AspNetCore.Http;

internal sealed class PaginationMiddleware : IMiddleware
{
    private readonly PaginationOptions _options;

    public PaginationMiddleware(IOptionsSnapshot<PaginationOptions>? options) =>
        this._options = options?.Value ?? new();

    Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);
        IQueryCollection query = context.Request.Query;

        if (
            query.TryGetValue("index", out StringValues indexString)
            && Int32.TryParse(indexString, out int index)
            && index >= 0
        )
        {
            this._options.PageIndex = index;
        }

        if (
            query.TryGetValue("count", out StringValues countString)
            && Int32.TryParse(countString, out int count)
            && count > 0
        )
        {
            this._options.MaxCount = count;
        }

        return next(context);
    }
}
