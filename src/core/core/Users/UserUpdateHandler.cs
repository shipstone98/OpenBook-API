using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Accounts;
using Shipstone.OpenBook.Api.Core.Services;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Users;

internal sealed class UserUpdateHandler : IUserUpdateHandler
{
    private readonly IClaimsService _claims;
    private readonly IRepository _repository;
    private readonly IValidationService _validation;

    public UserUpdateHandler(
        IRepository repository,
        IClaimsService claims,
        IValidationService validation
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(validation);
        this._claims = claims;
        this._repository = repository;
        this._validation = validation;
    }

    private async Task<IUser> HandleAsync(
        String forename,
        String surname,
        CancellationToken cancellationToken
    )
    {
        UserEntity user =
            await this._claims.RetrieveActiveUserAsync(
                this._repository,
                cancellationToken
            );

        user.Forename = forename;
        user.Surname = surname;
        user.Updated = DateTime.UtcNow;
        await this._repository.Users.UpdateAsync(user, cancellationToken);
        await this._repository.SaveAsync(cancellationToken);

        return await this._repository.RetrieveUserAsync(
            user,
            cancellationToken
        );
    }

    Task<IUser> IUserUpdateHandler.HandleAsync(
        String forename,
        String surname,
        CancellationToken cancellationToken
    )
    {
        forename = this._validation.ValidateForename(forename);
        surname = this._validation.ValidateSurname(surname);
        return this.HandleAsync(forename, surname, cancellationToken);
    }
}
