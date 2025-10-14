using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Identity;

using Shipstone.OpenBook.Api.Core.Services;
using Shipstone.OpenBook.Api.Infrastructure.Data.Repositories;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Core.Accounts;

internal sealed class AuthenticateHandler : IAuthenticateHandler
{
    private readonly IOtpService _otp;
    private readonly IPasswordService _password;
    private readonly IRepository _repository;

    public AuthenticateHandler(
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

    private async Task HandleAsync(
        String emailAddress,
        String password,
        CancellationToken cancellationToken
    )
    {
        UserEntity user =
            await this._repository.RetrieveActiveUserAsync(
                emailAddress,
                cancellationToken
            );

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

        await this._otp.GenerateAsync(user, cancellationToken);
    }

    Task IAuthenticateHandler.HandleAsync(
        String emailAddress,
        String password,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(password);
        return this.HandleAsync(emailAddress, password, cancellationToken);
    }
}
