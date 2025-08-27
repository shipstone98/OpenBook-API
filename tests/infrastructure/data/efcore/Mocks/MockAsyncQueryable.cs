using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;

internal class MockAsyncQueryable<T> : IAsyncEnumerable<T>, IQueryable<T>
{
    private readonly IEnumerable<T> _collection;
    private readonly IQueryProvider _provider;
    private readonly IQueryable _queryable;

    Type IQueryable.ElementType => throw new NotImplementedException();
    Expression IQueryable.Expression => this._queryable.Expression;
    IQueryProvider IQueryable.Provider => this._provider;

    internal MockAsyncQueryable(IQueryable<T> queryable)
    {
        this._collection = queryable;
        this._provider = new MockQueryProvider(queryable.Provider);
        this._queryable = queryable;
    }

    IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
    {
        IEnumerator<T> enumerator = this._collection.GetEnumerator();
        return new MockAsyncEnumerator<T>(enumerator);
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        throw new NotImplementedException();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        throw new NotImplementedException();
}
