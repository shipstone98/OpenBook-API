using System;
using Microsoft.Extensions.Options;

namespace Shipstone.OpenBook.Api.Infrastructure.Authentication;

/// <summary>
/// Specifies options for authentication requirements.
/// </summary>
public class AuthenticationOptions : IOptions<AuthenticationOptions>
{
    internal TimeSpan _otpExpiry;

    /// <summary>
    /// Gets or sets the expiry for one-time passcodes, expressed as minutes.
    /// </summary>
    /// <value>The expiry for one-time passcodes.</value>
    /// <exception cref="ArgumentOutOfRangeException">The property is set and the value is less than or equal to 0 (zero).</exception>
    public int OtpExpiryMinutes
    {
        get => (int) this._otpExpiry.TotalMinutes;

        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0);
            this._otpExpiry = TimeSpan.FromMinutes(value);
        }
    }

    AuthenticationOptions IOptions<AuthenticationOptions>.Value => this;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationOptions" /> class.
    /// </summary>
    public AuthenticationOptions() =>
        this._otpExpiry = TimeSpan.FromMinutes(10);
}
