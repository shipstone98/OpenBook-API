using System;

namespace Shipstone.OpenBook.Api.Infrastructure.Entities;

/// <summary>
/// Represents a user.
/// </summary>
public class UserEntity : Entity<Guid>
{
    private String _userName;

    /// <summary>
    /// Gets or sets the date and time the user last consented to policy.
    /// </summary>
    /// <value>The date and time the user last consented to policy.</value>
    public DateTime Consented { get; set; }

    /// <summary>
    /// Gets or sets the identity ID of the user.
    /// </summary>
    /// <value>The identity ID of the user.</value>
    public Guid IdentityId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is active.
    /// </summary>
    /// <value><c>true</c> if the user is active; otherwise, <c>false</c>.</value>
    public bool IsActive { get; set; }

    public String UserName
    {
        get => this._userName;

        set
        {
            ArgumentNullException.ThrowIfNull(value);
            this._userName = value;
        }
    }

    /// <summary>
    /// Gets or sets the normalized user name of the user.
    /// </summary>
    /// <value>The normalized user name of the user, if the user is active; otherwise, <c>null</c>.</value>
    public String? UserNameNormalized { get; set; }

    public UserEntity() => this._userName = String.Empty;
}
