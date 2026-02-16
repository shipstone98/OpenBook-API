using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.Extensions.Pagination;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;

internal sealed class AsyncQueryableService : IAsyncQueryableService
{
    Task<int> IAsyncQueryableService.CountAsync<TSource>(
        IQueryable<TSource> source,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.CountAsync(cancellationToken);
    }

    Task<List<TSource>> IAsyncQueryableService.ToListAsync<TSource>(
        IQueryable<TSource> source,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.ToListAsync(cancellationToken);
    }
}
