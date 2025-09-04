using System;
using System.Collections.Generic;
using System.Linq;

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
}
