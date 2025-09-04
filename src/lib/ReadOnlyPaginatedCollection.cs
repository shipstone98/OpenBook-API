using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Shipstone.Utilities.Collections;

public class ReadOnlyPaginatedCollection<T>
    : ReadOnlyCollection<T>, IReadOnlyPaginatedList<T>
{
    private static readonly ReadOnlyPaginatedCollection<T> _empty;

    private readonly int _pageCount;
    private readonly int _pageIndex;
    private readonly int _totalCount;

    public static ReadOnlyPaginatedCollection<T> Empty =>
        ReadOnlyPaginatedCollection<T>._empty;

    public int PageCount => this._pageCount;
    public int PageIndex => this._pageIndex;
    public int TotalCount => this._totalCount;

    static ReadOnlyPaginatedCollection()
    {
        IList<T> list = Array.Empty<T>();
        ReadOnlyPaginatedCollection<T>._empty = new(list, 0, 0, 1);
    }

    public ReadOnlyPaginatedCollection(
        IList<T> list,
        int totalCount,
        int pageIndex,
        int pageCount
    )
        : base(list)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(totalCount, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageIndex, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(pageCount, 0);

        if (pageCount < pageIndex)
        {
            throw new ArgumentException(
                $"{nameof (pageCount)} is less than {nameof (pageIndex)}.",
                nameof (pageCount)
            );
        }

        this._pageCount = pageCount;
        this._pageIndex = pageIndex;
        this._totalCount = totalCount;
    }
}
