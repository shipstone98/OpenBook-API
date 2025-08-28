using System.Collections.Generic;
using System.Threading;

namespace Shipstone.Test.Mocks;

public class MockAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    private readonly IEnumerable<T> _collection;

    public MockAsyncEnumerable(IEnumerable<T> collection) =>
        this._collection = collection;

    IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
    {
        IEnumerator<T> enumerator = this._collection.GetEnumerator();
        return new MockAsyncEnumerator<T>(enumerator);
    }
}
