using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Followings;

internal sealed class FollowingRetrieveHandler : IFollowingRetrieveHandler
{
    private readonly IClaimsService _claims;
    private readonly IRepository _repository;

    public FollowingRetrieveHandler(
        IRepository repository,
        IClaimsService claims
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(claims);
        this._claims = claims;
        this._repository = repository;
    }

    private async Task<IFollowing> HandleAsync(
        String userName,
        CancellationToken cancellationToken
    )
    {
        UserEntity followee =
            await this._repository.RetrieveActiveUserForNameAsync(
                userName,
                cancellationToken
            );

        Guid followerId = this._claims.Id;
        Guid followeeId = followee.Id;

        UserFollowingEntity? userFollowing =
            await this._repository.UserFollowings.RetrieveAsync(
                followerId,
                followeeId,
                cancellationToken
            );

        if (userFollowing is null)
        {
            throw new NotFoundException("The current user is not following the user whose name matches the provided user name.");
        }

        return new Following(
            this._claims.EmailAddress,
            followee.UserName,
            userFollowing.Followed,
            userFollowing.IsSubscribed
        );
    }

    Task<IFollowing> IFollowingRetrieveHandler.HandleAsync(
        String userName,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userName);
        return this.HandleAsync(userName, cancellationToken);
    }
}
