using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;

/// <summary>
/// Represents a data set.
/// </summary>
/// <typeparam name="TEntity">The type of entities in the data set.</typeparam>
public interface IDataSet<TEntity> : IQueryable<TEntity>
{
    /// <summary>
    /// Asynchronously sets the state of the specified entity.
    /// </summary>
    /// <param name="entity">The entity to set the state of.</param>
    /// <param name="state">The state for <c><paramref name="entity" /></c>.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous set operation.</returns>
    /// <exception cref="ArgumentException"><c><paramref name="state" /></c> is not one of the <see cref="DataEntityState" /> values.</exception>
    /// <exception cref="ArgumentNullException"><c><paramref name="entity" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SetStateAsync(
        TEntity entity,
        DataEntityState state,
        CancellationToken cancellationToken
    );
}
