using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Followings;

internal sealed class FollowingCreateHandler : IFollowingCreateHandler
{
    private readonly IClaimsService _claims;
    private readonly IRepository _repository;

    public FollowingCreateHandler(
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

        DateTime followed = DateTime.UtcNow;

        try
        {
            await this._repository.UserFollowings.CreateAsync(
                new UserFollowingEntity
                {
                    Followed = followed,
                    FolloweeId = followeeId,
                    FollowerId = followerId
                },
                cancellationToken
            );

            await this._repository.SaveAsync(cancellationToken);
        }

        catch (Exception ex)
        {
            throw new ConflictException(
                "The current user is already following the user whose name matches the provided user name.",
                ex
            );
        }

        return new Following(
            this._claims.EmailAddress,
            user.UserName,
            followed
        );
    }

    Task<IFollowing> IFollowingCreateHandler.HandleAsync(
        String userName,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userName);
        return this.HandleAsync(userName, cancellationToken);
    }
}
