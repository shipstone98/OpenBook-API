using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Authentication;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Accounts;

internal sealed class OtpAuthenticateHandler : IOtpAuthenticateHandler
{
    private readonly IAuthenticationService _authentication;
    private readonly IRepository _repository;

    public OtpAuthenticateHandler(
        IRepository repository,
        IAuthenticationService authentication
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(authentication);
        this._authentication = authentication;
        this._repository = repository;
    }

    private async Task<IAuthenticateResult> HandleAsync(
        String emailAddress,
        String otp,
        CancellationToken cancellationToken
    )
    {
        UserEntity user =
            await this._repository.RetrieveActiveUserAsync(
                emailAddress,
                cancellationToken
            );

        String? userOtp = user.Otp;
        Nullable<DateTime> userOtpExpires = user.OtpExpires;
        DateTime now = DateTime.UtcNow;

        if (userOtp is null || !String.Equals(userOtp, otp))
        {
            throw new ForbiddenException("The provided OTP does not match the OTP for the user whose email address matches the provided email address.");
        }

        Nullable<DateTime> otpExpires = userOtpExpires;
        user.Otp = null;
        user.OtpExpires = null;
        await this._repository.Users.UpdateAsync(user, cancellationToken);
        await this._repository.SaveAsync(cancellationToken);

        if (
            !otpExpires.HasValue
            || DateTime.Compare(now, otpExpires.Value) > 0
        )
        {
            throw new ForbiddenException("The provided OTP has expired.");
        }

        Guid userId = user.Id;

        IAsyncEnumerable<String> roles =
            await this._repository.RetrieveRolesAsync(
                userId,
                cancellationToken
            );

        IEnumerable<String> roleCollection =
            roles.ToBlockingEnumerable(cancellationToken);

        IAuthenticateResult result =
            await this._authentication.AuthenticateAsync(
                user,
                roleCollection,
                now,
                cancellationToken
            );

        await this._repository.UserRefreshTokens.CreateAsync(
            new UserRefreshTokenEntity
            {
                Created = now,
                Expires = result.RefreshTokenExpires,
                Updated = now,
                UserId = userId,
                Value = result.RefreshToken
            },
            cancellationToken
        );

        await this._repository.SaveAsync(cancellationToken);
        return result;
    }

    Task<IAuthenticateResult> IOtpAuthenticateHandler.HandleAsync(
        String emailAddress,
        String otp,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(otp);
        return this.HandleAsync(emailAddress, otp, cancellationToken);
    }
}
