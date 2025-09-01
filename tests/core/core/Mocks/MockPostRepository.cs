using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.CoreTest.Mocks;

internal sealed class MockPostRepository : IPostRepository
{
    internal Action<PostEntity> _createAction;
    internal Func<long, PostEntity?> _retrieveFunc;

    internal MockPostRepository()
    {
        this._createAction = _ => throw new NotImplementedException();
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

    Task<PostEntity?> IPostRepository.RetrieveAsync(
        long id,
        CancellationToken cancellationToken
    )
    {
        PostEntity? result = this._retrieveFunc(id);
        return Task.FromResult(result);
    }
}
