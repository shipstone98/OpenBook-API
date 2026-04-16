using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Security;
using Shipstone.Utilities;

using Shipstone.OpenBook.Api.Core.Services;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Mail;

namespace Shipstone.OpenBook.Api.Core.Accounts;

internal sealed class RegisterHandler : IRegisterHandler
{
    private readonly IMailService _mail;
    private readonly INormalizationService _normalization;
    private readonly IRepository _repository;
    private readonly IValidationService _validation;

    public RegisterHandler(
        IRepository repository,
        IMailService mail,
        INormalizationService normalization,
        IValidationService validation
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(mail);
        ArgumentNullException.ThrowIfNull(normalization);
        ArgumentNullException.ThrowIfNull(validation);
        this._mail = mail;
        this._normalization = normalization;
        this._repository = repository;
        this._validation = validation;
    }

    private async Task<IUser> HandleAsync(
        Guid identityId,
        String userName,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        UserEntity user = new UserEntity
        {
            Consented = now,
            Created = now,
            IdentityId = identityId,
            IsActive = true,
            Updated = now,
            UserName = userName,
            UserNameNormalized = this._normalization.Normalize(userName)
        };

        await this._repository.Users.CreateAsync(user, cancellationToken);

        await this._repository.UserRoles.CreateAsync(
            new UserRoleEntity
            {
                Assigned = now,
                RoleId = Roles.UserId,
                UserId = user.Id
            },
            cancellationToken
        );

        try
        {
            await this._repository.SaveAsync(cancellationToken);
        }

        catch (Exception ex)
        {
            throw new ConflictException(
                "A user whose identity ID and/or name matches the identity ID of the current user already exists -or- a user whose name matches the provided user name already exists.",
                ex
            );
        }

        await this._mail.SendRegistrationAsync(cancellationToken);
        IReadOnlySet<String> roles = new SortedSet<String> { Roles.User };
        return new User(user, roles);
    }

    Task<IUser> IRegisterHandler.HandleAsync(
        Guid identityId,
        String userName,
        CancellationToken cancellationToken
    )
    {
        if (Guid.Equals(identityId, Guid.Empty))
        {
            throw new ArgumentException(
                $"{nameof (identityId)} is equal to Guid.Empty.",
                nameof (identityId)
            );
        }

        userName = this._validation.ValidateUserName(userName);
        DateTime now = DateTime.UtcNow;
        return this.HandleAsync(identityId, userName, now, cancellationToken);
    }
}
