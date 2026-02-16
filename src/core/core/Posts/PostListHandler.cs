using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Collections;
using Shipstone.Utilities.Linq;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Posts;

internal sealed class PostListHandler : IPostListHandler
{
    private readonly IClaimsService _claims;
    private readonly IRepository _repository;

    public PostListHandler(IRepository repository, IClaimsService claims)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(claims);
        this._claims = claims;
        this._repository = repository;
    }

    private async Task<IReadOnlyPaginatedList<IPost>> HandleAsync(
        long parentId,
        CancellationToken cancellationToken
    )
    {
        PostEntity? parent =
            await this._repository.Posts.RetrieveAsync(
                parentId,
                cancellationToken
            );

        if (parent is null)
        {
            throw new NotFoundException("A post whose ID matches the provided parent ID could not be found.");
        }

        IReadOnlyPaginatedList<PostEntity> posts =
            await this._repository.Posts.ListForParentAsync(
                parent.Id,
                cancellationToken
            );

        return await posts.SelectAsync(
            (p, _, ct) =>
                this._repository.RetrievePostAsync(this._claims, p, ct),
            cancellationToken
        );
    }

    private async Task<IReadOnlyPaginatedList<IPost>> HandleAsync(
        String userName,
        CancellationToken cancellationToken
    )
    {
        UserEntity? user =
            await this._repository.Users.RetrieveForNameAsync(
                userName,
                cancellationToken
            );

        if (user is null)
        {
            throw new NotFoundException("A user whose name matches the provided user name could not be found.");
        }

        IReadOnlyPaginatedList<PostEntity> posts =
            await this._repository.Posts.ListForCreatorAsync(
                user.Id,
                cancellationToken
            );

        return posts.Select((p, _) =>
            new Post(p, user.EmailAddress, user.UserName));
    }

    Task<IReadOnlyPaginatedList<IPost>> IPostListHandler.HandleAsync(
        long parentId,
        CancellationToken cancellationToken
    )
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(parentId, 0);
        return this.HandleAsync(parentId, cancellationToken);
    }

    Task<IReadOnlyPaginatedList<IPost>> IPostListHandler.HandleAsync(
        String userName,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userName);
        return this.HandleAsync(userName, cancellationToken);
    }
}
