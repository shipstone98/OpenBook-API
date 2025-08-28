using System;

namespace Shipstone.OpenBook.Api.Infrastructure.Entities;

/// <summary>
/// Represents a user refresh token.
/// </summary>
public class UserRefreshTokenEntity : Entity<long>
{
    private String _value;

    /// <summary>
    /// Gets or initializes the date and time the user refresh token will expire.
    /// </summary>
    /// <value>The date and time the user refresh token will expire.</value>
    public DateTime Expires { get; init; }

    /// <summary>
    /// Gets the ID of the associated user.
    /// </summary>
    /// <value>A <see cref="Guid" /> containing the ID of the associated user.
    public Guid UserId { get; init; }

    public String Value
    {
        get => this._value;

        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._value = value;
        }
    }

    public UserRefreshTokenEntity() => this._value = String.Empty;
}
