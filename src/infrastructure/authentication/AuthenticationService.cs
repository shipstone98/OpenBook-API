using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Shipstone.Utilities.Security.Cryptography;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationOptions _options;
    private readonly TokenValidationParameters _refreshTokenValidationParameters;
    private readonly RandomNumberGenerator _rng;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public AuthenticationService(
        RandomNumberGenerator rng,
        JwtSecurityTokenHandler tokenHandler,
        IOptions<AuthenticationOptions>? options
    )
    {
        ArgumentNullException.ThrowIfNull(rng);
        ArgumentNullException.ThrowIfNull(tokenHandler);
        AuthenticationOptions optionsValue = options?.Value ?? new();

        TokenValidationParameters refreshTokenValidationParameters =
            new TokenValidationParameters
            {
                IssuerSigningKey = optionsValue._refreshTokenSigningKey.Key,
                ValidAudience = optionsValue._audience,
                ValidIssuer = optionsValue._issuer,
                ValidateIssuerSigningKey = true
            };

        this._options = optionsValue;

        this._refreshTokenValidationParameters =
            refreshTokenValidationParameters;

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

        ICollection<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.EmailAddress),
            new(ClaimTypes.GivenName, user.Forename),
            new(ClaimTypes.Name, user.UserName),
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
        String otp = this._rng.GenerateOtp(Constants.UserOtpMaxLength);
        DateTime otpExpires = now.Add(this._options._otpExpiry);
        user.Otp = otp;
        user.OtpExpires = otpExpires;
        user.Updated = now;
        return Task.CompletedTask;
    }

    Guid IAuthenticationService.GetId(String token)
    {
        ArgumentNullException.ThrowIfNull(token);

        ClaimsPrincipal principal =
            this._tokenHandler.ValidateToken(
                token,
                this._refreshTokenValidationParameters,
                out _
            );

        Claim? claim =
            principal.Claims.FirstOrDefault(c =>
                c.Type.Equals(ClaimTypes.NameIdentifier));

        if (claim is null || !Guid.TryParse(claim.Value, out Guid id))
        {
            throw new ArgumentException(
                "The provided token is not a valid token.",
                nameof (token)
            );
        }

        return id;
    }
}
