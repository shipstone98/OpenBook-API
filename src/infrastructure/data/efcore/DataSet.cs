using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;

internal sealed class DataSet<TEntity> : IDataSet<TEntity> where TEntity : class
{
    private readonly IQueryable<TEntity> _query;
    private readonly DbSet<TEntity> _set;

    Type IQueryable.ElementType => this._query.ElementType;
    Expression IQueryable.Expression => this._query.Expression;
    IQueryProvider IQueryable.Provider => this._query.Provider;

    internal DataSet(DbSet<TEntity> dbSet)
    {
        this._query = dbSet.AsNoTracking();
        this._set = dbSet;
    }

    Task IDataSet<TEntity>.SetStateAsync(
        TEntity entity,
        DataEntityState state,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (!Enum.IsDefined(state))
        {
            throw new ArgumentException(
                $"{nameof (state)} is not one of the DataEntityState values.",
                nameof (state)
            );
        }

        EntityEntry<TEntity> entry = this._set.Entry(entity);

        entry.State = state switch
        {
            DataEntityState.Created => EntityState.Added,
            DataEntityState.Updated => EntityState.Modified,
            DataEntityState.Deleted => EntityState.Deleted,
            _ => throw new NotImplementedException()
        };

        return Task.CompletedTask;
    }

    IEnumerator IEnumerable.GetEnumerator() => this._query.GetEnumerator();

    IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() =>
        this._query.GetEnumerator();
}
