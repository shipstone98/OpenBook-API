using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Posts;

internal sealed class PostRetrieveHandler : IPostRetrieveHandler
{
    private readonly IClaimsService _claims;
    private readonly IRepository _repository;

    public PostRetrieveHandler(IRepository repository, IClaimsService claims)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(claims);
        this._claims = claims;
        this._repository = repository;
    }

    private async Task<IPost> HandleAsync(
        long id,
        CancellationToken cancellationToken
    )
    {
        PostEntity? post =
            await this._repository.Posts.RetrieveAsync(id, cancellationToken);

        if (post is null)
        {
            throw new NotFoundException("A post whose ID matches the provided ID could not be found.");
        }

        return await this._repository.RetrievePostAsync(
            this._claims,
            post,
            cancellationToken
        );
    }

    Task<IPost> IPostRetrieveHandler.HandleAsync(
        long id,
        CancellationToken cancellationToken
    )
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);
        return this.HandleAsync(id, cancellationToken);
    }
}
