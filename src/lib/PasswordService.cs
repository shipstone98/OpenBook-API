using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Shipstone.Extensions.Identity;

internal sealed class PasswordService : IPasswordService
{
    private readonly IPasswordHasher<IPasswordService> _hasher;
    private readonly PasswordOptions _options;

    public PasswordService(
        IPasswordHasher<IPasswordService> hasher,
        IOptions<PasswordOptions>? options
    )
    {
        ArgumentNullException.ThrowIfNull(hasher);
        this._hasher = hasher;
        this._options = options?.Value ?? new();
    }

    String IPasswordService.Hash(String password)
    {
        ArgumentNullException.ThrowIfNull(password);
        return this._hasher.HashPassword(this, password);
    }

    void IPasswordService.Validate(String password)
    {
        ArgumentNullException.ThrowIfNull(password);

        IDictionary<PasswordRequirements, Func<String, bool>> predicates =
            new Dictionary<PasswordRequirements, Func<String, bool>>
        {
            {
                PasswordRequirements.RequiredLength,
                p => p.Length >= this._options.RequiredLength
            },
            {
                PasswordRequirements.RequiredUniqueChars,
                p =>
                {
                    IReadOnlySet<char> characters = new HashSet<char>(p);
                    return characters.Count >= this._options.RequiredUniqueChars;
                }
            }
        };

        if (this._options.RequireDigit)
        {
            predicates.Add(
                PasswordRequirements.RequireDigit,
                p => p.Any(Char.IsDigit)
            );
        }

        if (this._options.RequireLowercase)
        {
            predicates.Add(
                PasswordRequirements.RequireLowercase,
                p => p.Any(Char.IsLower)
            );
        }

        if (this._options.RequireNonAlphanumeric)
        {
            predicates.Add(
                PasswordRequirements.RequireNonAlphanumeric,
                p => p.Any(c => !Char.IsLetterOrDigit(c))
            );
        }

        if (this._options.RequireUppercase)
        {
            predicates.Add(
                PasswordRequirements.RequireUppercase,
                p => p.Any(Char.IsUpper)
            );
        }

        PasswordRequirements failedRequirements = PasswordRequirements.None;

        foreach (KeyValuePair<PasswordRequirements, Func<String, bool>> predicate in predicates)
        {
            if (!predicate.Value(password))
            {
                failedRequirements |= predicate.Key;
            }
        }

        if (failedRequirements != PasswordRequirements.None)
        {
            throw new PasswordNotValidException(
                nameof (password),
                failedRequirements,
                $"{nameof (password)} is not a valid password."
            );
        }
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
