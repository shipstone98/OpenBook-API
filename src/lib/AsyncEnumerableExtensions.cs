using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.Utilities.Linq;

public static class AsyncEnumerableExtensions
{
    public static IAsyncEnumerable<TSource> WithoutNullAsync<TSource>(
        this IAsyncEnumerable<TSource?> source,
        CancellationToken cancellationToken = default
    )
        where TSource : class
    {
        ArgumentNullException.ThrowIfNull(source);

        return AsyncEnumerableExtensions.WithoutNullAsyncCore(
            source,
            cancellationToken
        );
    }

    private static async IAsyncEnumerable<TSource> WithoutNullAsyncCore<TSource>(
        this IAsyncEnumerable<TSource?> source,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await foreach (TSource? item in source.WithCancellation(cancellationToken))
        {
            if (item is not null)
            {
                yield return item;
            }
        }
    }
}
