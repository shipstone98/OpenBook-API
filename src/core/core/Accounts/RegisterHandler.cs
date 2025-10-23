using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.OpenBook.Api.Core.Services;
using Shipstone.OpenBook.Api.Core.Users;
using Shipstone.OpenBook.Api.Infrastructure.Authentication;
using Shipstone.OpenBook.Api.Infrastructure.Data;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Mail;

namespace Shipstone.OpenBook.Api.Core.Accounts;

internal sealed class RegisterHandler : IRegisterHandler
{
    private readonly IAuthenticationService _authentication;
    private readonly IMailService _mail;
    private readonly INormalizationService _normalization;
    private readonly IRepository _repository;
    private readonly IValidationService _validation;

    public RegisterHandler(
        IRepository repository,
        IAuthenticationService authentication,
        IMailService mail,
        INormalizationService normalization,
        IValidationService validation
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(authentication);
        ArgumentNullException.ThrowIfNull(mail);
        ArgumentNullException.ThrowIfNull(normalization);
        ArgumentNullException.ThrowIfNull(validation);
        this._authentication = authentication;
        this._mail = mail;
        this._normalization = normalization;
        this._repository = repository;
        this._validation = validation;
    }

    private async Task<IUser> HandleAsync(
        String emailAddress,
        String userName,
        String forename,
        String surname,
        DateOnly born,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        bool isCreated;

        UserEntity? user =
            await this._repository.Users.RetrieveAsync(
                emailAddress,
                cancellationToken
            );

        if (user is null)
        {
            user = new UserEntity
            {
                Born = born,
                Created = now,
                Consented = now,
                EmailAddress = emailAddress,
                EmailAddressNormalized =
                    this._normalization.Normalize(emailAddress),
                Forename = forename,
                IsActive = true,
                Surname = surname,
                Updated = now,
                UserName = userName,
                UserNameNormalized = this._normalization.Normalize(userName)
            };

            isCreated = true;
        }

        else if (user.IsActive && user.PasswordHash is null)
        {
            user.Consented = now;
            user.Updated = now;
            isCreated = false;
        }

        else
        {
            throw new ConflictException("A user whose email address and/or name matches the provided email address and/or user name already exists.");
        }

        await this._authentication.GenerateOtpAsync(
            user,
            now,
            cancellationToken
        );

        if (isCreated)
        {
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
        }

        else
        {
            await this._repository.Users.UpdateAsync(user, cancellationToken);
        }

        await this._repository.SaveAsync(cancellationToken);
        TimeSpan difference = user.OtpExpires!.Value.Subtract(now);

        await this._mail.SendRegistrationAsync(
            user,
            (int) difference.TotalMinutes,
            cancellationToken
        );

        IReadOnlySet<String> roles = new SortedSet<String> { Roles.User };
        return new User(user, roles);
    }

    Task<IUser> IRegisterHandler.HandleAsync(
        String emailAddress,
        String userName,
        String forename,
        String surname,
        DateOnly born,
        CancellationToken cancellationToken
    )
    {
        emailAddress = this._validation.ValidateEmailAddress(emailAddress);
        userName = this._validation.ValidateUserName(userName);
        forename = this._validation.ValidateForename(forename);
        surname = this._validation.ValidateSurname(surname);
        DateTime now = DateTime.UtcNow;
        DateOnly today = DateOnly.FromDateTime(now);
        this._validation.ValidateBorn(born, today);

        return this.HandleAsync(
            emailAddress,
            userName,
            forename,
            surname,
            born,
            now,
            cancellationToken
        );
    }
}
