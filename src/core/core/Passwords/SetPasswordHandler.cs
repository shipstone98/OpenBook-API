using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Identity;

using Shipstone.OpenBook.Api.Core.Services;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Passwords;

internal sealed class SetPasswordHandler : ISetPasswordHandler
{
    private readonly IOtpService _otp;
    private readonly IPasswordService _password;
    private readonly IRepository _repository;

    public SetPasswordHandler(
        IRepository repository,
        IOtpService otp,
        IPasswordService password
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(otp);
        ArgumentNullException.ThrowIfNull(password);
        this._otp = otp;
        this._password = password;
        this._repository = repository;
    }

    async Task<IUser> ISetPasswordHandler.HandleAsync(
        String emailAddress,
        String otp,
        String password,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(otp);
        this._password.Validate(password);

        UserEntity? user =
            await this._repository.RetrieveActiveUserAsync(
                emailAddress,
                cancellationToken
            );

        DateTime now = DateTime.UtcNow;
        await this._otp.ValidateOtpAsync(user, otp, now, cancellationToken);
        user.PasswordHash = this._password.Hash(password);
        user.Updated = now;
        await this._repository.Users.UpdateAsync(user, cancellationToken);
        await this._repository.SaveAsync(cancellationToken);

        return await this._repository.RetrieveUserAsync(
            user,
            cancellationToken
        );
    }
}
