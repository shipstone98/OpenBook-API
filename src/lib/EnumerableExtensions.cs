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

    internal static async IAsyncEnumerable<TResult> SelectAsyncCore<TSource, TResult>(
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

    public static IAsyncEnumerable<TResult> SelectManyAsync<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, CancellationToken, Task<IEnumerable<TResult>>> selector,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        return EnumerableExtensions.SelectManyAsyncCore(
            source,
            selector,
            cancellationToken
        );
    }

    private static async IAsyncEnumerable<TResult> SelectManyAsyncCore<TSource, TResult>(
        IEnumerable<TSource> source,
        Func<TSource, CancellationToken, Task<IEnumerable<TResult>>> selector,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        foreach (TSource item in source)
        {
            foreach (TResult resultItem in await selector(item, cancellationToken))
            {
                yield return resultItem;
            }
        }
    }
}
