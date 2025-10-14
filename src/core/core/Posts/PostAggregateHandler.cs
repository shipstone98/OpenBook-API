using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Collections;
using Shipstone.Utilities.Linq;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Posts;

internal sealed class PostAggregateHandler : IPostAggregateHandler
{
    private readonly IClaimsService _claims;
    private readonly IRepository _repository;

    public PostAggregateHandler(IRepository repository, IClaimsService claims)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(claims);
        this._claims = claims;
        this._repository = repository;
    }

    async Task<IReadOnlyPaginatedList<IPost>> IPostAggregateHandler.HandleAsync(CancellationToken cancellationToken)
    {
        IEnumerable<UserFollowingEntity> userFollowings =
            await this._repository.UserFollowings.ListForFollowerAsync(
                this._claims.Id,
                cancellationToken
            );

        if (!userFollowings.Any())
        {
            return ReadOnlyPaginatedCollection<IPost>.Empty;
        }

        IEnumerable<Guid> followeeIds =
            userFollowings.Select(uf => uf.FolloweeId);

        IReadOnlyPaginatedList<PostEntity> posts =
            await this._repository.Posts.ListForCreatorsAsync(
                followeeIds,
                cancellationToken
            );

        return await posts.SelectAsync(
            (p, ct) => this._repository.RetrievePostAsync(this._claims, p, ct),
            cancellationToken
        );
    }
}
