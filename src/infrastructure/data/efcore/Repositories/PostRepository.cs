using System;
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

        IDataSet<PostEntity> dataSet = this._dataSource.Posts;

        IQueryable<PostEntity> query =
            dataSet
                .Where(p => Guid.Equals(creatorId, p.CreatorId))
                .OrderByDescending(p => p.Created);

        return this._pagination.GetPageOrFirstAsync(
            dataSet,
            query,
            cancellationToken
        );
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
