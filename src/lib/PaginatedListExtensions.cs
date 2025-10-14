using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Collections;

namespace Shipstone.Utilities.Linq;

public static class PaginatedListExtensions
{
    public static IReadOnlyPaginatedList<TResult> Select<TSource, TResult>(
        this IReadOnlyPaginatedList<TSource> source,
        Func<TSource, TResult> selector
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        IList<TResult> list =
            (source as IEnumerable<TSource>)
                .Select(selector)
                .ToArray();

        return new ReadOnlyPaginatedCollection<TResult>(
            list,
            source.TotalCount,
            source.PageIndex,
            source.PageCount
        );
    }

    public static Task<IReadOnlyPaginatedList<TResult>> SelectAsync<TSource, TResult>(
        this IReadOnlyPaginatedList<TSource> source,
        Func<TSource, CancellationToken, Task<TResult>> selector,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        return PaginatedListExtensions.SelectAsyncCore(
            source,
            selector,
            cancellationToken
        );
    }

    private static async Task<IReadOnlyPaginatedList<TResult>> SelectAsyncCore<TSource, TResult>(
        IReadOnlyPaginatedList<TSource> source,
        Func<TSource, CancellationToken, Task<TResult>> selector,
        CancellationToken cancellationToken
    )
    {
        IList<TResult> list =
            await source
                .SelectAsyncCore(selector, cancellationToken)
                .ToListAsync(cancellationToken);

        return new ReadOnlyPaginatedCollection<TResult>(
            list,
            source.TotalCount,
            source.PageIndex,
            source.PageCount
        );
    }
}
