using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;

internal class MockAsyncQueryable<T> : MockAsyncEnumerable<T>, IQueryable<T>
{
    private readonly IQueryProvider _provider;
    private readonly IQueryable _queryable;

    Type IQueryable.ElementType => throw new NotImplementedException();
    Expression IQueryable.Expression => this._queryable.Expression;
    IQueryProvider IQueryable.Provider => this._provider;

    internal MockAsyncQueryable(IQueryable<T> queryable) : base(queryable)
    {
        this._provider = new MockAsyncQueryProvider(queryable.Provider);
        this._queryable = queryable;
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        throw new NotImplementedException();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        throw new NotImplementedException();
}
