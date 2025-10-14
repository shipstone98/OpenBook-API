using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities.Linq;

using Shipstone.OpenBook.Api.Core.Services;
using Shipstone.OpenBook.Api.Infrastructure.Authentication;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Accounts;

internal sealed class OtpAuthenticateHandler : IOtpAuthenticateHandler
{
    private readonly IAuthenticationService _authentication;
    private readonly IOtpService _otp;
    private readonly IRepository _repository;

    public OtpAuthenticateHandler(
        IRepository repository,
        IAuthenticationService authentication,
        IOtpService otp
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(authentication);
        ArgumentNullException.ThrowIfNull(otp);
        this._authentication = authentication;
        this._otp = otp;
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

        DateTime now = DateTime.UtcNow;
        await this._otp.ValidateOtpAsync(user, otp, now, cancellationToken);
        Guid userId = user.Id;

        IAsyncEnumerable<String> roles =
            await this._repository.RetrieveRolesAsync(
                userId,
                cancellationToken
            );

        IEnumerable<String> roleCollection =
            await roles.ToListAsync(cancellationToken);

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
