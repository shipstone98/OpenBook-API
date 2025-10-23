using System;

using Shipstone.OpenBook.Api.Core;

namespace Shipstone.OpenBook.Api.Infrastructure.Entities;

/// <summary>
/// Represents a user.
/// </summary>
public class UserEntity : Entity<Guid>
{
    private String _emailAddress;
    private String _emailAddressNormalized;
    private String _forename;
    private String? _otp;
    private String _surname;
    private String _userName;
    private String _userNameNormalized;

    /// <summary>
    /// Gets or initializes the date the user was born.
    /// </summary>
    /// <value>The date the user was born.</value>
    public DateOnly Born { get; init; }

    /// <summary>
    /// Gets or sets the date and time the user last consented to policy.
    /// </summary>
    /// <value>The date and time the user last consented to policy.</value>
    public DateTime Consented { get; set; }

    public String EmailAddress
    {
        get => this._emailAddress;

        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._emailAddress = value;
        }
    }

    public String EmailAddressNormalized
    {
        get => this._emailAddressNormalized;

        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._emailAddressNormalized = value;
        }
    }

    public String Forename
    {
        get => this._forename;

        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._forename = value;
        }
    }

    /// <summary>
    /// Gets or initializes a value indicating whether the user is active.
    /// </summary>
    /// <value><c>true</c> if the user is active; otherwise, <c>false</c>.</value>
    public bool IsActive { get; init; }

    public String? Otp
    {
        get => this._otp;

        set
        {
            if (value is null)
            {
                this._otp = null;
            }

            else if (value.Length > Constants.UserOtpMaxLength)
            {
                throw new ArgumentException(
                    $"The length of {nameof (value)} is greater than Constants.UserOtpMaxLength.",
                    nameof (value)
                );
            }

            else
            {
                this._otp = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the date and time the OTP (one-time passcode) of the user will expire.
    /// </summary>
    /// <value>The date and time the OTP (one-time passcode) of the user will expire, or <c>null</c> if <see cref="UserEntity.Otp" /> is <c>null</c>.</value>
    public Nullable<DateTime> OtpExpires { get; set; }

    /// <summary>
    /// Gets or sets the password hash of the user.
    /// </summary>
    /// <value>The password hash of the user, or <c>null</c>.</value>
    public String? PasswordHash { get; set; }

    public String Surname
    {
        get => this._surname;

        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._surname = value;
        }
    }

    public String UserName
    {
        get => this._userName;

        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._userName = value;
        }
    }

    public String UserNameNormalized
    {
        get => this._userNameNormalized;

        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._userNameNormalized = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserEntity" /> class.
    /// </summary>
    public UserEntity()
    {
        this._emailAddress = String.Empty;
        this._emailAddressNormalized = String.Empty;
        this._forename = String.Empty;
        this._surname = String.Empty;
        this._userName = String.Empty;
        this._userNameNormalized = String.Empty;
    }
}
