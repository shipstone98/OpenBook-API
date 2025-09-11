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
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(repository);
        this._claims = claims;
        this._repository = repository;
    }

    private async Task<IReadOnlyPaginatedList<IPost>> HandleAsync(
        Guid userId,
        String userEmailAddress,
        String userName,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyPaginatedList<PostEntity> posts =
            await this._repository.Posts.ListForCreatorAsync(
                userId,
                cancellationToken
            );

        return posts.Select(p => new Post(p, userEmailAddress, userName));
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

        return await this.HandleAsync(
            user.Id,
            user.EmailAddress,
            user.UserName,
            cancellationToken
        );
    }

    Task<IReadOnlyPaginatedList<IPost>> IPostListHandler.HandleAsync(CancellationToken cancellationToken) =>
        this.HandleAsync(
            this._claims.Id,
            this._claims.EmailAddress,
            this._claims.UserName,
            cancellationToken
        );

    Task<IReadOnlyPaginatedList<IPost>> IPostListHandler.HandleAsync(
        String userName,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userName);
        return this.HandleAsync(userName, cancellationToken);
    }
}
