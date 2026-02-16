using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Mail;

namespace Shipstone.OpenBook.Api.Core.Accounts;

internal sealed class UnregisterHandler : IUnregisterHandler
{
    private readonly IClaimsService _claims;
    private readonly IMailService _mail;
    private readonly IRepository _repository;

    public UnregisterHandler(
        IRepository repository,
        IClaimsService claims,
        IMailService mail
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(mail);
        this._claims = claims;
        this._mail = mail;
        this._repository = repository;
    }

    private async Task DeleteUserFollowingsAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<UserFollowingEntity> userFollowings =
            await this._repository.UserFollowings.ListForFolloweeAsync(
                userId,
                cancellationToken
            );

        foreach (UserFollowingEntity userFollowing in userFollowings)
        {
            await this._repository.UserFollowings.DeleteAsync(
                userFollowing,
                cancellationToken
            );
        }

        userFollowings =
            await this._repository.UserFollowings.ListForFollowerAsync(
                userId,
                cancellationToken
            );

        foreach (UserFollowingEntity userFollowing in userFollowings)
        {
            await this._repository.UserFollowings.DeleteAsync(
                userFollowing,
                cancellationToken
            );
        }
    }

    private async Task DeleteUserDevicesAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<UserDeviceEntity> userDevices =
            await this._repository.UserDevices.ListForUserAsync(
                userId,
                cancellationToken
            );

        foreach (UserDeviceEntity userDevice in userDevices)
        {
            await this._repository.UserDevices.DeleteAsync(
                userDevice,
                cancellationToken
            );
        }
    }

    private async Task DeleteUserRefreshTokensAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<UserRefreshTokenEntity> userRefreshTokens =
            await this._repository.UserRefreshTokens.ListForUserAsync(
                userId,
                cancellationToken
            );

        foreach (UserRefreshTokenEntity userRefreshToken in userRefreshTokens)
        {
            await this._repository.UserRefreshTokens.DeleteAsync(
                userRefreshToken,
                cancellationToken
            );
        }
    }

    private async Task DeleteUserRolesAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<UserRoleEntity> userRoles =
            await this._repository.UserRoles.ListForUserAsync(
                userId,
                cancellationToken
            );

        foreach (UserRoleEntity userRole in userRoles)
        {
            await this._repository.UserRoles.DeleteAsync(
                userRole,
                cancellationToken
            );
        }
    }

    async Task IUnregisterHandler.HandleAsync(CancellationToken cancellationToken)
    {
        UserEntity user =
            await this._claims.RetrieveActiveUserAsync(
                this._repository,
                cancellationToken
            );

        user.Born = DateOnly.MinValue;
        user.EmailAddress = String.Empty;
        user.EmailAddressNormalized = null;
        user.Forename = String.Empty;
        user.IsActive = false;
        user.Otp = null;
        user.OtpExpires = null;
        user.PasswordHash = null;
        user.Surname = String.Empty;
        user.Updated = DateTime.UtcNow;
        user.UserName = String.Empty;
        user.UserNameNormalized = null;
        Guid userId = user.Id;
        await this.DeleteUserDevicesAsync(userId, cancellationToken);
        await this.DeleteUserFollowingsAsync(userId, cancellationToken);
        await this.DeleteUserRefreshTokensAsync(userId, cancellationToken);
        await this.DeleteUserRolesAsync(userId, cancellationToken);
        await this._repository.Users.UpdateAsync(user, cancellationToken);
        await this._repository.SaveAsync(cancellationToken);

        await this._mail.SendUnregistrationAsync(
            this._claims.EmailAddress,
            cancellationToken
        );
    }
}
