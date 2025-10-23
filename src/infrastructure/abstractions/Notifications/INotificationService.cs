using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Notifications;

/// <summary>
/// Defines methods to send push notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Asynchronously sends a <c>Post Created</c> notification to the specified user devices.
    /// </summary>
    /// <param name="creatorName">The name of the user that created the post.</param>
    /// <param name="id">The ID of the post.</param>
    /// <param name="userDevices">A collection containing the user devices to send the notification to.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous send operation.</returns>
    /// <exception cref="ArgumentException"><c><paramref name="userDevices" /></c> contains one or more elements that are <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"><c><paramref name="creatorName" /></c> is <c>null</c> -or- <c><paramref name="userDevices" /></c> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="id" /></c> is less than or equal to 0 (zero).</exception>
    /// <exception cref="NotificationException">The notifications could not be sent.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SendPostCreatedAsync(
        String creatorName,
        long id,
        IEnumerable<UserDeviceEntity> userDevices,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously sends a <c>User Followed</c> notification to the specified user devices.
    /// </summary>
    /// <param name="userName">The name of the user that was followed.</param>
    /// <param name="userDevices">A collection containing the user devices to send the notification to.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous send operation.</returns>
    /// <exception cref="ArgumentException"><c><paramref name="userDevices" /></c> contains one or more elements that are <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"><c><paramref name="userName" /></c> is <c>null</c> -or- <c><paramref name="userDevices" /></c> is <c>null</c>.</exception>
    /// <exception cref="NotificationException">The notifications could not be sent.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SendUserFollowedAsync(
        String userName,
        IEnumerable<UserDeviceEntity> userDevices,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously sends a <c>User Unfollowed</c> notification to the specified user devices.
    /// </summary>
    /// <param name="userName">The name of the user that was unfollowed.</param>
    /// <param name="userDevices">A collection containing the user devices to send the notification to.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous send operation.</returns>
    /// <exception cref="ArgumentException"><c><paramref name="userDevices" /></c> contains one or more elements that are <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"><c><paramref name="userName" /></c> is <c>null</c> -or- <c><paramref name="userDevices" /></c> is <c>null</c>.</exception>
    /// <exception cref="NotificationException">The notifications could not be sent.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SendUserUnfollowedAsync(
        String userName,
        IEnumerable<UserDeviceEntity> userDevices,
        CancellationToken cancellationToken
    );
}
