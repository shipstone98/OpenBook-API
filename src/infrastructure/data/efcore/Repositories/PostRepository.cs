using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.Utilities.Collections;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Services;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Repositories;

internal sealed class PostRepository : IPostRepository
{
    private readonly IDataSource _dataSource;
    private readonly IPaginationService _pagination;

    public PostRepository(
        IDataSource dataSource,
        IPaginationService pagination
    )
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        ArgumentNullException.ThrowIfNull(pagination);
        this._dataSource = dataSource;
        this._pagination = pagination;
    }

    private Task ModifyAsync(
        PostEntity post,
        DataEntityState state,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(post);

        return this._dataSource.Posts.SetStateAsync(
            post,
            state,
            cancellationToken
        );
    }

    Task IPostRepository.CreateAsync(
        PostEntity post,
        CancellationToken cancellationToken
    ) =>
        this.ModifyAsync(post, DataEntityState.Created, cancellationToken);

    Task IPostRepository.DeleteAsync(
        PostEntity post,
        CancellationToken cancellationToken
    ) =>
        this.ModifyAsync(post, DataEntityState.Deleted, cancellationToken);

#warning Not tested
    Task<IReadOnlyPaginatedList<PostEntity>> IPostRepository.ListForCreatorAsync(
        Guid creatorId,
        CancellationToken cancellationToken
    )
    {
        if (Guid.Equals(creatorId, Guid.Empty))
        {
            throw new ArgumentException(
                $"{nameof (creatorId)} is equal to Guid.Empty.",
                nameof (creatorId)
            );
        }

        IQueryable<PostEntity> query =
            this._dataSource.Posts
                .Where(p => Guid.Equals(creatorId, p.CreatorId))
                .OrderByDescending(p => p.Created);

        return this._pagination.GetPageOrFirstAsync(query, cancellationToken);
    }

#warning Not tested
    Task<IReadOnlyPaginatedList<PostEntity>> IPostRepository.ListForCreatorsAsync(
        IEnumerable<Guid> creatorIds,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(creatorIds);

        if (creatorIds.Any(id => Guid.Equals(id, Guid.Empty)))
        {
            throw new ArgumentException(
                $"{nameof (creatorIds)} contains one or more elements that are equal to Guid.Empty.",
                nameof (creatorIds)
            );
        }

        IQueryable<PostEntity> query =
            this._dataSource.Posts
                .Where(p => creatorIds.Contains(p.CreatorId))
                .OrderByDescending(p => p.Created);

        return this._pagination.GetPageOrFirstAsync(query, cancellationToken);
    }

    Task<PostEntity?> IPostRepository.RetrieveAsync(
        long id,
        CancellationToken cancellationToken
    )
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);

        return this._dataSource.Posts.FirstOrDefaultAsync(
            p => id == p.Id,
            cancellationToken
        );
    }
}
