using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Identity;

using Shipstone.OpenBook.Api.Infrastructure.Authentication;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;
using Shipstone.OpenBook.Api.Infrastructure.Mail;

namespace Shipstone.OpenBook.Api.Core.Accounts;

internal sealed class AuthenticateHandler : IAuthenticateHandler
{
    private readonly IAuthenticationService _authentication;
    private readonly IMailService _mail;
    private readonly IPasswordService _password;
    private readonly IRepository _repository;

    public AuthenticateHandler(
        IRepository repository,
        IAuthenticationService authentication,
        IMailService mail,
        IPasswordService password
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(authentication);
        ArgumentNullException.ThrowIfNull(mail);
        ArgumentNullException.ThrowIfNull(password);
        this._authentication = authentication;
        this._mail = mail;
        this._password = password;
        this._repository = repository;
    }

    async Task IAuthenticateHandler.HandleAsync(
        String emailAddress,
        String password,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(password);
        IUserRepository userRepository = this._repository.Users;

        UserEntity? user =
            await userRepository.RetrieveAsync(
                emailAddress,
                cancellationToken
            );

        if (user is null)
        {
            throw new NotFoundException("A user whose email address matches the provided email address could not be found.");
        }

        if (!user.IsActive)
        {
            throw new UserNotActiveException("The user whose email address matches the provided email address is not active.");
        }

        String? passwordHash = user.PasswordHash;

        if (passwordHash is null)
        {
            throw new ForbiddenException("The user whose email address matches the provided email address has not verified their email address.");
        }

        bool isPasswordSecure;

        try
        {
            isPasswordSecure = this._password.Verify(passwordHash, password);
        }

        catch (IncorrectPasswordException ex)
        {
            throw new IncorrectPasswordException(
                "The hashed representation of the provided password does not match the password hash of the user whose email address matches the provided email address.",
                ex
            );
        }

        if (!isPasswordSecure)
        {
            user.PasswordHash = this._password.Hash(password);
        }

        DateTime now = DateTime.UtcNow;

        await this._authentication.GenerateOtpAsync(
            user,
            now,
            cancellationToken
        );

        user.Updated = now;
        await userRepository.UpdateAsync(user, cancellationToken);
        await this._repository.SaveAsync(cancellationToken);
        TimeSpan difference = user.OtpExpires!.Value.Subtract(now);

        await this._mail.SendOtpAsync(
            user,
            (int) difference.TotalMinutes,
            cancellationToken
        );
    }
}
