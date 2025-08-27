using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationOptions _options;
    private readonly RandomNumberGenerator _rng;

    public AuthenticationService(
        RandomNumberGenerator rng,
        IOptions<AuthenticationOptions>? options
    )
    {
        ArgumentNullException.ThrowIfNull(rng);
        this._rng = rng;
        this._options = options?.Value ?? new();
    }

    private String GenerateOtp()
    {
        const int BYTE_COUNT = Constants.UserOtpMaxLength;
        Span<byte> bytes = stackalloc byte[6];
        this._rng.GetNonZeroBytes(bytes);

        for (int i = 0; i < BYTE_COUNT; i ++)
        {
            bytes[i] = (byte) ((bytes[i] % 9) + '1');
        }

        return Encoding.ASCII.GetString(bytes);
    }

    Task IAuthenticationService.GenerateOtpAsync(
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(user);
        String otp = this.GenerateOtp();
        DateTime otpExpires = now.Add(this._options._otpExpiry);
        user.Otp = otp;
        user.OtpExpires = otpExpires;
        user.Updated = now;
        return Task.CompletedTask;
    }
}
