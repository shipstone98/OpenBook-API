using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Identity;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Passwords;

internal sealed class PasswordUpdateHandler : IPasswordUpdateHandler
{
    private readonly IClaimsService _claims;
    private readonly IPasswordService _password;
    private readonly IRepository _repository;

    public PasswordUpdateHandler(
        IRepository repository,
        IClaimsService claims,
        IPasswordService password
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(password);
        this._claims = claims;
        this._password = password;
        this._repository = repository;
    }

    private async Task<IUser> HandleAsync(
        String passwordCurrent,
        String passwordNew,
        CancellationToken cancellationToken
    )
    {
        UserEntity user =
            await this._claims.RetrieveActiveUserAsync(
                this._repository,
                cancellationToken
            );

        this._password.Verify(user, passwordCurrent);
        user.PasswordHash = this._password.Hash(passwordNew);
        user.Updated = DateTime.UtcNow;
        await this._repository.Users.UpdateAsync(user, cancellationToken);
        await this._repository.SaveAsync(cancellationToken);

        return await this._repository.RetrieveUserAsync(
            user,
            cancellationToken
        );
    }

    Task<IUser> IPasswordUpdateHandler.HandleAsync(
        String passwordCurrent,
        String passwordNew,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(passwordCurrent);
        this._password.Validate(passwordNew);

        return this.HandleAsync(
            passwordCurrent,
            passwordNew,
            cancellationToken
        );
    }
}
