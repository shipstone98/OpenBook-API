using System;

using Shipstone.OpenBook.Api.Core;

namespace Shipstone.OpenBook.Api.Infrastructure.Entities;

/// <summary>
/// Represents a role.
/// </summary>
public class RoleEntity : Entity<long>
{
    private String _name;
    private String _nameNormalized;

    public String Name
    {
        get => this._name;

        init
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value.Length > Constants.RoleNameMaxLength)
            {
                throw new ArgumentException(
                    $"The length of {nameof (value)} is greater than Constants.RoleNameMaxLength.",
                    nameof (value)
                );
            }

            this._name = value;
        }
    }

    public String NameNormalized
    {
        get => this._nameNormalized;

        init
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value.Length > Constants.RoleNameMaxLength)
            {
                throw new ArgumentException(
                    $"The length of {nameof (value)} is greater than Constants.RoleNameMaxLength.",
                    nameof (value)
                );
            }

            this._nameNormalized = value;
        }
    }

    public RoleEntity()
    {
        this._name = String.Empty;
        this._nameNormalized = String.Empty;
    }
}
