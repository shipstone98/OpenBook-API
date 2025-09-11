using System;

using Shipstone.Extensions.Notifications;

namespace Shipstone.OpenBook.Api.Infrastructure.Entities;

/// <summary>
/// Represents a user device.
/// </summary>
public class UserDeviceEntity : Entity<long>, INotificationConsumer
{
    private String _token;
    private String _type;

    public String Token
    {
        get => this._token;

        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._token = value;
        }
    }

    public String Type
    {
        get => this._type;

        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._type = value;
        }
    }

    /// <summary>
    /// Gets or initializes the ID of the associated user.
    /// </summary>
    /// <value>A <see cref="Guid" /> containing the ID of the associated user.</value>
    public Guid UserId { get; init; }

    public UserDeviceEntity()
    {
        this._token = String.Empty;
        this._type = String.Empty;
    }
}
