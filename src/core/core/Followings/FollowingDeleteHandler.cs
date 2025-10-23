using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Followings;

internal sealed class FollowingDeleteHandler : IFollowingDeleteHandler
{
    private readonly IClaimsService _claims;
    private readonly IRepository _repository;

    public FollowingDeleteHandler(
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
        UserEntity? user =
            await this._repository.Users.RetrieveForNameAsync(
                userName,
                cancellationToken
            );

        if (user is null)
        {
            throw new NotFoundException("A user whose name matches the provided user name could not be found.");
        }

        if (!user.IsActive)
        {
            throw new UserNotActiveException("The user whose name matches the provided user name is not active.");
        }

        Guid followerId = this._claims.Id;
        Guid followeeId = user.Id;

        if (Guid.Equals(followerId, followeeId))
        {
            throw new ForbiddenException("The name of the current user matches the provided user name.");
        }

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

        await this._repository.UserFollowings.DeleteAsync(
            userFollowing,
            cancellationToken
        );

        await this._repository.SaveAsync(cancellationToken);

        return new Following(
            this._claims.EmailAddress,
            user.UserName,
            userFollowing.Followed
        );
    }

    Task<IFollowing> IFollowingDeleteHandler.HandleAsync(
        String userName,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userName);
        return this.HandleAsync(userName, cancellationToken);
    }
}
