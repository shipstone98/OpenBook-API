using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Repositories;

internal sealed class PostRepository : IPostRepository
{
    private readonly IDataSource _dataSource;

    public PostRepository(IDataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        this._dataSource = dataSource;
    }

    Task IPostRepository.CreateAsync(
        PostEntity post,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(post);

        return this._dataSource.Posts.SetStateAsync(
            post,
            DataEntityState.Created,
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
