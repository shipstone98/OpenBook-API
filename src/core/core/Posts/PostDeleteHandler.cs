using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Authorization;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Posts;

internal sealed class PostDeleteHandler : IPostDeleteHandler
{
    private readonly IAuthorizationService _authorization;
    private readonly IClaimsService _claims;
    private readonly IRepository _repository;

    public PostDeleteHandler(
        IRepository repository,
        IAuthorizationService authorization,
        IClaimsService claims
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(authorization);
        ArgumentNullException.ThrowIfNull(claims);
        this._authorization = authorization;
        this._claims = claims;
        this._repository = repository;
    }

    private async Task<IPost> HandleAsync(
        long id,
        String policy,
        CancellationToken cancellationToken
    )
    {
        IPostRepository posts = this._repository.Posts;

        PostEntity? postEntity =
            await posts.RetrieveAsync(id, cancellationToken);

        if (postEntity is null)
        {
            throw new NotFoundException("A post whose ID matches the provided ID could not be found.");
        }

        IResource postResource =
            await this._repository.RetrieveResourceAsync(
                postEntity.CreatorId,
                cancellationToken
            );

        await this._authorization.AuthorizeAsync(
            postResource,
            policy,
            "The current user is not authorized to delete the post whose ID matches the provided ID.",
            cancellationToken
        );

        IPost post =
            await this._repository.RetrievePostAsync(
                this._claims,
                postEntity,
                cancellationToken
            );

        await posts.DeleteAsync(postEntity, cancellationToken);
        await this._repository.SaveAsync(cancellationToken);
        postEntity.Updated = DateTime.UtcNow;
        return post;
    }

    Task<IPost> IPostDeleteHandler.HandleAsync(
        long id,
        String policy,
        CancellationToken cancellationToken
    )
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);
        ArgumentNullException.ThrowIfNull(policy);
        return this.HandleAsync(id, policy, cancellationToken);
    }
}
