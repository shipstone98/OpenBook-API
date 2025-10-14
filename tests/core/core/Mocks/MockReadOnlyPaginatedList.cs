using System;
using System.Collections;
using System.Collections.Generic;

namespace Shipstone.Utilities.Collections;

internal sealed class MockReadOnlyPaginatedList<T> : IReadOnlyPaginatedList<T>
{
    internal Func<IEnumerator<T>> _getEnumeratorFunc;
    internal Func<int> _pageCountFunc;
    internal Func<int> _pageIndexFunc;
    internal Func<int> _totalCountFunc;

    int IReadOnlyCollection<T>.Count => throw new NotImplementedException();
    int IReadOnlyPaginatedList<T>.PageCount => this._pageCountFunc();
    int IReadOnlyPaginatedList<T>.PageIndex => this._pageIndexFunc();
    int IReadOnlyPaginatedList<T>.TotalCount => this._totalCountFunc();

    T IReadOnlyList<T>.this[int index] => throw new NotImplementedException();

    internal MockReadOnlyPaginatedList()
    {
        this._getEnumeratorFunc = () => throw new NotImplementedException();
        this._pageCountFunc = () => throw new NotImplementedException();
        this._pageIndexFunc = () => throw new NotImplementedException();
        this._totalCountFunc = () => throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        throw new NotImplementedException();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this._getEnumeratorFunc();
}
