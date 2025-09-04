using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Shipstone.Extensions.Pagination;
using Shipstone.Utilities.Collections;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Services;

internal sealed class PaginationService : IPaginationService
{
    private readonly PaginationOptions _options;

    public PaginationService(IOptionsSnapshot<PaginationOptions>? options) =>
        this._options = options?.Value ?? new();

    async Task<IReadOnlyPaginatedList<T>> IPaginationService.GetPageOrFirstAsync<T>(
        IDataSet<T> dataSet,
        IQueryable<T> query,
        CancellationToken cancellationToken
    )
    {
        int totalCount = await dataSet.CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return ReadOnlyPaginatedCollection<T>.Empty;
        }

        int pageIndex = this._options.PageIndex;
        int maxCount = this._options.MaxCount;

        IList<T> list =
            await PaginationService.GetAsync(
                query,
                pageIndex,
                maxCount,
                cancellationToken
            );

        if (list.Count == 0)
        {
            list =
                await PaginationService.GetAsync(
                    query,
                    0,
                    maxCount,
                    cancellationToken
                );
        }

        int pageCount = (int) Math.Ceiling(totalCount / (double) maxCount);
        pageCount = Math.Max(pageCount, 1);

        return new ReadOnlyPaginatedCollection<T>(
            list,
            totalCount,
            pageIndex,
            pageCount
        );
    }

    private static Task<T[]> GetAsync<T>(
        IQueryable<T> query,
        int pageIndex,
        int maxCount,
        CancellationToken cancellationToken
    ) =>
        query
            .Skip(pageIndex * maxCount)
            .Take(maxCount)
            .ToArrayAsync(cancellationToken);
}
