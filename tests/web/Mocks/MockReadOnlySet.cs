using System;
using System.Collections;
using System.Collections.Generic;

namespace Shipstone.OpenBook.Api.WebTest.Mocks;

internal sealed class MockReadOnlySet<T> : IReadOnlySet<T>
{
    internal Func<T, bool> _containsFunc;

    int IReadOnlyCollection<T>.Count => throw new NotImplementedException();

    internal MockReadOnlySet() =>
        this._containsFunc = _ => throw new NotImplementedException();

    bool IReadOnlySet<T>.Contains(T item) => this._containsFunc(item);

    IEnumerator IEnumerable.GetEnumerator() =>
        throw new NotImplementedException();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        throw new NotImplementedException();

    bool IReadOnlySet<T>.IsProperSubsetOf(IEnumerable<T> other) =>
        throw new NotImplementedException();

    bool IReadOnlySet<T>.IsProperSupersetOf(IEnumerable<T> other) =>
        throw new NotImplementedException();

    bool IReadOnlySet<T>.IsSubsetOf(IEnumerable<T> other) =>
        throw new NotImplementedException();

    bool IReadOnlySet<T>.IsSupersetOf(IEnumerable<T> other) =>
        throw new NotImplementedException();

    bool IReadOnlySet<T>.Overlaps(IEnumerable<T> other) =>
        throw new NotImplementedException();

    bool IReadOnlySet<T>.SetEquals(IEnumerable<T> other) =>
        throw new NotImplementedException();
}
