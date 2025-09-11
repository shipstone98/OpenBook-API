using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.Utilities.Linq;

public static class AsyncEnumerableExtensions
{
    public static Task<List<TSource>> ToListAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(source);

        return AsyncEnumerableExtensions.ToListAsyncCore(
            source,
            cancellationToken
        );
    }

    private static async Task<List<TSource>> ToListAsyncCore<TSource>(
        IAsyncEnumerable<TSource> source,
        CancellationToken cancellationToken
    )
    {
        List<TSource> list = new();

        await foreach (TSource item in source.WithCancellation(cancellationToken))
        {
            list.Add(item);
        }

        return list;
    }

    public static Task<SortedSet<TSource>> ToSortedSetAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        IComparer<TSource>? comparer = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(source);

        return AsyncEnumerableExtensions.ToSortedSetAsyncCore(
            source,
            comparer,
            cancellationToken
        );
    }

    private static async Task<SortedSet<TSource>> ToSortedSetAsyncCore<TSource>(
        IAsyncEnumerable<TSource> source,
        IComparer<TSource>? comparer,
        CancellationToken cancellationToken
    )
    {
        SortedSet<TSource> sortedSet = new(comparer);

        await foreach (TSource item in source.WithCancellation(cancellationToken))
        {
            sortedSet.Add(item);
        }

        return sortedSet;
    }

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
        [EnumeratorCancellation] CancellationToken cancellationToken
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
