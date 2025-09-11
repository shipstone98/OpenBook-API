using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Linq;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Notifications;

namespace Shipstone.OpenBook.Api.Core;

internal static class NotificationServiceExtensions
{
    internal static async Task NotifyAllSubscribedFollowersAsync(
        this INotificationService notification,
        IRepository repository,
        Guid followeeId,
        String followeeName,
        long postId,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<UserFollowingEntity> userFollowings =
            await repository.UserFollowings.ListForFolloweeAsync(
                followeeId,
                cancellationToken
            );

        IEnumerable<UserDeviceEntity> userDevices =
            await userFollowings
                .Where(uf => uf.IsSubscribed)
                .SelectManyAsync<UserFollowingEntity, UserDeviceEntity>(
                    async (uf, ct) =>
                        await repository.UserDevices.ListForUserAsync(
                            uf.FollowerId,
                            ct
                        ),
                    cancellationToken
                )
                .ToListAsync(cancellationToken);

        await notification.SendPostCreatedAsync(
            followeeName,
            postId,
            userDevices,
            cancellationToken
        );
    }
}
