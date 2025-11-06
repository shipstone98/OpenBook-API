using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Notifications;

namespace Shipstone.OpenBook.Api.Core.Followings;

internal sealed class FollowingCreateHandler : IFollowingCreateHandler
{
    private readonly IClaimsService _claims;
    private readonly INotificationService _notification;
    private readonly IRepository _repository;

    public FollowingCreateHandler(
        IRepository repository,
        IClaimsService claims,
        INotificationService notification
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(notification);
        this._claims = claims;
        this._notification = notification;
        this._repository = repository;
    }

    private async Task<IFollowing> HandleAsync(
        String userName,
        bool isSubscribed,
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
                    FollowerId = followerId,
                    IsSubscribed = isSubscribed
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

        await this._notification.NotifyUserFollowedAsync(
            this._repository,
            followee,
            cancellationToken
        );

        return new Following(
            this._claims.EmailAddress,
            followee.UserName,
            followed,
            isSubscribed
        );
    }

    Task<IFollowing> IFollowingCreateHandler.HandleAsync(
        String userName,
        bool isSubscribed,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userName);
        return this.HandleAsync(userName, isSubscribed, cancellationToken);
    }
}
