using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Collections;
using Shipstone.Utilities.Linq;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Posts;

internal sealed class PostListHandler : IPostListHandler
{
    private readonly IRepository _repository;

    public PostListHandler(IRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this._repository = repository;
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

        return posts.Select(p =>
            new Post(p, user.EmailAddress, user.UserName));
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
