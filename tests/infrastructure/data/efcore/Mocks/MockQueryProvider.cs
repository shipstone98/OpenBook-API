using System;
using System.Linq;
using System.Linq.Expressions;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;

internal sealed class MockQueryProvider : IQueryProvider
{
    private readonly IQueryProvider _provider;

    internal MockQueryProvider(IQueryProvider provider) =>
        this._provider = provider;

    IQueryable IQueryProvider.CreateQuery(Expression expression) =>
        throw new NotImplementedException();

    IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
    {
        IQueryable<TElement> queryable =
            this._provider.CreateQuery<TElement>(expression);

        return new MockAsyncQueryable<TElement>(queryable);
    }

    Object? IQueryProvider.Execute(Expression expression) =>
        throw new NotImplementedException();

    TResult IQueryProvider.Execute<TResult>(Expression expression) =>
        throw new NotImplementedException();
}
