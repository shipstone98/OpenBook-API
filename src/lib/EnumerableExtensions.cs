using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.Utilities.Linq;

public static class EnumerableExtensions
{
    public static IAsyncEnumerable<TResult> SelectAsync<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, CancellationToken, Task<TResult>> selector,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        return EnumerableExtensions.SelectAsyncCore(
            source,
            selector,
            cancellationToken
        );
    }

    private static async IAsyncEnumerable<TResult> SelectAsyncCore<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, CancellationToken, Task<TResult>> selector,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        foreach (TSource item in source)
        {
            yield return await selector(item, cancellationToken);
        }
    }
}
