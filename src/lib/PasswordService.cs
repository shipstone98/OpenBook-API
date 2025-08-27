using System;
using Microsoft.AspNetCore.Identity;

namespace Shipstone.Extensions.Identity;

internal sealed class PasswordService : IPasswordService
{
    private readonly IPasswordHasher<IPasswordService> _hasher;

    public PasswordService(IPasswordHasher<IPasswordService> hasher)
    {
        ArgumentNullException.ThrowIfNull(hasher);
        this._hasher = hasher;
    }

    String IPasswordService.Hash(String password)
    {
        ArgumentNullException.ThrowIfNull(password);
        return this._hasher.HashPassword(this, password);
    }

    bool IPasswordService.Verify(String passwordHash, String password)
    {
        ArgumentNullException.ThrowIfNull(passwordHash);
        ArgumentNullException.ThrowIfNull(password);

        PasswordVerificationResult result =
            this._hasher.VerifyHashedPassword(this, passwordHash, password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new IncorrectPasswordException("The provided password hash does not match the hashed representation of the provided password.");
        }

        return result == PasswordVerificationResult.Success;
    }
}
