using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shipstone.Test.Mocks;

public sealed class MockAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _enumerator;

    T IAsyncEnumerator<T>.Current => this._enumerator.Current;

    public MockAsyncEnumerator(IEnumerator<T> enumerator) =>
        this._enumerator = enumerator;

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        this._enumerator.Dispose();
        return ValueTask.CompletedTask;
    }

    ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync()
    {
        bool result = this._enumerator.MoveNext();
        return ValueTask.FromResult(result);
    }
}
