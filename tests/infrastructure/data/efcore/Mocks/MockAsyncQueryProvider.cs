using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCoreTest.Mocks;

internal sealed class MockAsyncQueryProvider : IAsyncQueryProvider
{
    private readonly IQueryProvider _provider;

    internal MockAsyncQueryProvider(IQueryProvider provider) =>
        this._provider = provider;

    TResult IAsyncQueryProvider.ExecuteAsync<TResult>(
        Expression expression,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<Type> types = typeof (TResult).GetGenericArguments();
        Object? result = this._provider.Execute(expression);
        Object?[]? arguments = new Object?[1] { result };

        Object? obj =
            typeof (Task)
                .GetMethod(nameof (Task.FromResult))?
                .MakeGenericMethod(types[0])
                .Invoke(null, arguments);

        if (obj is not TResult invokeResult)
        {
            throw new NotImplementedException();
        }

        return invokeResult;
    }

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
