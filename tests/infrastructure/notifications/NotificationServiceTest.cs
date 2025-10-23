using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Notifications;

using Shipstone.OpenBook.Api.Infrastructure.NotificationsTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.OpenBook.Api.Infrastructure.NotificationsTest;

public sealed class NotificationServiceTest
{
    private readonly INotificationService _notification;
    private readonly MockNotificationService _mockNotification;

    public NotificationServiceTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddOpenBookInfrastructureNotifications();
        MockNotificationService notification = new();
        services.AddSingleton<Extensions.Notifications.INotificationService>(notification);
        IServiceProvider provider = new MockServiceProvider(services);

        this._notification =
            provider.GetRequiredService<INotificationService>();

        this._mockNotification = notification;
    }

#region SendPostCreatedAsync method
#region Invalid arguments
    [Fact]
    public async Task TestSendPostCreatedAsync_Invalid_CreatorNameNull()
    {
        // Arrange
        IEnumerable<UserDeviceEntity> userDevices =
            Array.Empty<UserDeviceEntity>();

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._notification.SendPostCreatedAsync(
                    null!,
                    1,
                    userDevices,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("creatorName", ex.ParamName);
    }

    [InlineData(Int64.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [Theory]
    public async Task TestSendPostCreatedAsync_Invalid_IdInvalid(long id)
    {
        // Arrange
        IEnumerable<UserDeviceEntity> userDevices =
            Array.Empty<UserDeviceEntity>();

        // Act
        ArgumentOutOfRangeException ex =
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                this._notification.SendPostCreatedAsync(
                    String.Empty,
                    id,
                    userDevices,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal(id, ex.ActualValue);
        Assert.Equal("id", ex.ParamName);
    }

    [Fact]
    public async Task TestSendPostCreatedAsync_Invalid_UserDevicesContainsNull()
    {
        // Arrange
        IEnumerable<UserDeviceEntity> userDevices =
            new UserDeviceEntity[1] { null! };

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._notification.SendPostCreatedAsync(
                    String.Empty,
                    1,
                    userDevices,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("userDevices", ex.ParamName);
    }

    [Fact]
    public async Task TestSendPostCreatedAsync_Invalid_UserDevicesNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._notification.SendPostCreatedAsync(
                    String.Empty,
                    1,
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("userDevices", ex.ParamName);
    }
#endregion

    [Fact]
    public async Task TestSendPostCreatedAsync_Valid_Failure()
    {
        // Arrange
        Exception innerException =
            new Extensions.Notifications.NotificationException();

        IEnumerable<UserDeviceEntity> userDevices =
            Array.Empty<UserDeviceEntity>();

        this._mockNotification._sendAction = (_, _) => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<NotificationException>(() =>
                this._notification.SendPostCreatedAsync(
                    String.Empty,
                    1,
                    userDevices,
                    CancellationToken.None
                ));

        // Assert
        Assert.Same(innerException, ex.InnerException);
    }

    [InlineData(1)]
    [InlineData(Int64.MaxValue)]
    [Theory]
    public Task TestSendPostCreatedAsync_Valid_Success(long id)
    {
        // Arrange
        IEnumerable<UserDeviceEntity> userDevices =
            Array.Empty<UserDeviceEntity>();

        this._mockNotification._sendAction = (_, _) => { };

        // Act
        return this._notification.SendPostCreatedAsync(
            String.Empty,
            id,
            userDevices,
            CancellationToken.None
        );

        // Nothing to assert
    }
#endregion

#region SendUserFollowedAsync method
#region Invalid arguments
    [Fact]
    public async Task TestSendUserFollowedAsync_Invalid_UserDevicesContainsNull()
    {
        // Arrange
        IEnumerable<UserDeviceEntity> userDevices =
            new UserDeviceEntity[1] { null! };

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._notification.SendUserFollowedAsync(
                    String.Empty,
                    userDevices,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("userDevices", ex.ParamName);
    }

    [Fact]
    public async Task TestSendUserFollowedAsync_Invalid_UserDevicesNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._notification.SendUserFollowedAsync(
                    String.Empty,
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("userDevices", ex.ParamName);
    }

    [Fact]
    public async Task TestSendUserFollowedAsync_Invalid_UserNameNull()
    {
        // Arrange
        IEnumerable<UserDeviceEntity> userDevices =
            Array.Empty<UserDeviceEntity>();

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._notification.SendUserFollowedAsync(
                    null!,
                    userDevices,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("userName", ex.ParamName);
    }
#endregion

    [Fact]
    public async Task TestSendUserFollowedAsync_Valid_Failure()
    {
        // Arrange
        Exception innerException =
            new Extensions.Notifications.NotificationException();

        IEnumerable<UserDeviceEntity> userDevices =
            Array.Empty<UserDeviceEntity>();

        this._mockNotification._sendAction = (_, _) => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<NotificationException>(() =>
                this._notification.SendUserFollowedAsync(
                    String.Empty,
                    userDevices,
                    CancellationToken.None
                ));

        // Assert
        Assert.Same(innerException, ex.InnerException);
    }

    [Fact]
    public Task TestSendUserFollowedAsync_Valid_Success()
    {
        // Arrange
        IEnumerable<UserDeviceEntity> userDevices =
            Array.Empty<UserDeviceEntity>();

        this._mockNotification._sendAction = (_, _) => { };

        // Act
        return this._notification.SendUserFollowedAsync(
            String.Empty,
            userDevices,
            CancellationToken.None
        );

        // Nothing to assert
    }
#endregion

#region SendUserUnfollowedAsync method
#region Invalid arguments
    [Fact]
    public async Task TestSendUserUnfollowedAsync_Invalid_UserDevicesContainsNull()
    {
        // Arrange
        IEnumerable<UserDeviceEntity> userDevices =
            new UserDeviceEntity[1] { null! };

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._notification.SendUserUnfollowedAsync(
                    String.Empty,
                    userDevices,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("userDevices", ex.ParamName);
    }

    [Fact]
    public async Task TestSendUserUnfollowedAsync_Invalid_UserDevicesNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._notification.SendUserUnfollowedAsync(
                    String.Empty,
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("userDevices", ex.ParamName);
    }

    [Fact]
    public async Task TestSendUserUnfollowedAsync_Invalid_UserNameNull()
    {
        // Arrange
        IEnumerable<UserDeviceEntity> userDevices =
            Array.Empty<UserDeviceEntity>();

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._notification.SendUserUnfollowedAsync(
                    null!,
                    userDevices,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("userName", ex.ParamName);
    }
#endregion

    [Fact]
    public async Task TestSendUserUnfollowedAsync_Valid_Failure()
    {
        // Arrange
        Exception innerException =
            new Extensions.Notifications.NotificationException();

        IEnumerable<UserDeviceEntity> userDevices =
            Array.Empty<UserDeviceEntity>();

        this._mockNotification._sendAction = (_, _) => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<NotificationException>(() =>
                this._notification.SendUserUnfollowedAsync(
                    String.Empty,
                    userDevices,
                    CancellationToken.None
                ));

        // Assert
        Assert.Same(innerException, ex.InnerException);
    }

    [Fact]
    public Task TestSendUserUnfollowedAsync_Valid_Success()
    {
        // Arrange
        IEnumerable<UserDeviceEntity> userDevices =
            Array.Empty<UserDeviceEntity>();

        this._mockNotification._sendAction = (_, _) => { };

        // Act
        return this._notification.SendUserUnfollowedAsync(
            String.Empty,
            userDevices,
            CancellationToken.None
        );

        // Nothing to assert
    }
#endregion
}
