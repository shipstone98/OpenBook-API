using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationOptions _options;
    private readonly RandomNumberGenerator _rng;
    private readonly SecurityTokenHandler _tokenHandler;

    public AuthenticationService(
        RandomNumberGenerator rng,
        SecurityTokenHandler tokenHandler,
        IOptions<AuthenticationOptions>? options
    )
    {
        ArgumentNullException.ThrowIfNull(rng);
        ArgumentNullException.ThrowIfNull(tokenHandler);
        this._options = options?.Value ?? new();
        this._rng = rng;
        this._tokenHandler = tokenHandler;
    }

    private String GenerateAccessToken(
        UserEntity user,
        IEnumerable<String> roles,
        DateTime now
    )
    {
        roles = new HashSet<String>(roles);
        String id = user.Id.ToString();
        String emailAddress = user.EmailAddress;

        ICollection<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.Email, emailAddress),
            new(ClaimTypes.GivenName, user.Forename),
            new(ClaimTypes.Name, emailAddress),
            new(ClaimTypes.NameIdentifier, id),
            new(ClaimTypes.Surname, user.Surname)
        };

        foreach (String role in roles)
        {
            Claim claim = new(ClaimTypes.Role, role);
            claims.Add(claim);
        }

        SecurityToken token =
            this._tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Audience = this._options._audience,
                Expires = now.Add(this._options._accessTokenExpiry),
                IssuedAt = now,
                Issuer = this._options._issuer,
                NotBefore = now,
                SigningCredentials = this._options._accessTokenSigningKey,
                Subject = new(claims)
            });

        return this._tokenHandler.WriteToken(token);
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

    private (String Value, DateTime Expires) GenerateRefreshToken(UserEntity user, DateTime now)
    {
        String id = user.Id.ToString();

        IEnumerable<Claim> claims =
            new Claim[] { new(ClaimTypes.NameIdentifier, id) };

        DateTime expires = now.Add(this._options._refreshTokenExpiry);

        SecurityToken token =
            this._tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Audience = this._options._audience,
                Expires = expires,
                IssuedAt = now,
                Issuer = this._options._issuer,
                NotBefore = now,
                SigningCredentials = this._options._refreshTokenSigningKey,
                Subject = new(claims)
            });

        String val = this._tokenHandler.WriteToken(token);
        return (val, expires);
    }

    Task<IAuthenticateResult> IAuthenticationService.AuthenticateAsync(
        UserEntity user,
        IEnumerable<String> roles,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roles);

        if (roles.Any(r => r is null))
        {
            throw new ArgumentException(
                $"{nameof (roles)} contains one or more elements that are null.",
                nameof (roles)
            );
        }

        String accessToken = this.GenerateAccessToken(user, roles, now);

        (String refreshToken, DateTime refreshTokenExpires) =
            this.GenerateRefreshToken(user, now);

        IAuthenticateResult result =
            new AuthenticateResult(
                accessToken,
                refreshToken,
                refreshTokenExpires
            );

        return Task.FromResult(result);
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
