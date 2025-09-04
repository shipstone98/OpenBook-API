using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Collections;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockPostRepository : IPostRepository
{
    internal Action<PostEntity> _createAction;
    internal Action<PostEntity> _deleteAction;

    internal Func<Guid, IReadOnlyPaginatedList<PostEntity>> _listFunc;
    internal Func<long, PostEntity?> _retrieveFunc;

    internal MockPostRepository()
    {
        this._createAction = _ => throw new NotImplementedException();
        this._deleteAction = _ => throw new NotImplementedException();
        this._listFunc = _ => throw new NotImplementedException();
        this._retrieveFunc = _ => throw new NotImplementedException();
    }

    Task IPostRepository.CreateAsync(
        PostEntity post,
        CancellationToken cancellationToken
    )
    {
        this._createAction(post);
        return Task.CompletedTask;
    }

    Task IPostRepository.DeleteAsync(
        PostEntity post,
        CancellationToken cancellationToken
    )
    {
        this._deleteAction(post);
        return Task.CompletedTask;
    }

    Task<IReadOnlyPaginatedList<PostEntity>> IPostRepository.ListAsync(
        Guid creatorId,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyPaginatedList<PostEntity> result = this._listFunc(creatorId);
        return Task.FromResult(result);
    }

    Task<PostEntity?> IPostRepository.RetrieveAsync(
        long id,
        CancellationToken cancellationToken
    )
    {
        PostEntity? result = this._retrieveFunc(id);
        return Task.FromResult(result);
    }
}
