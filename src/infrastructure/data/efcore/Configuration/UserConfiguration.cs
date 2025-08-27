using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shipstone.Extensions.Security;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Configuration;

internal sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    private readonly IEncryptionService _encryption;

    internal UserConfiguration(IEncryptionService encryption) =>
        this._encryption = encryption;

    void IEntityTypeConfiguration<UserEntity>.Configure(EntityTypeBuilder<UserEntity> builder)
    {
        IEnumerable<Expression<Func<UserEntity, String?>>> protectedProperties =
            new Expression<Func<UserEntity, String?>>[]
        {
            u => u.EmailAddress,
            u => u.Forename,
            u => u.Surname,
            u => u.UserName
        };

        foreach (Expression<Func<UserEntity, String?>> protectedProperty in protectedProperties)
        {
            builder
                .Property(protectedProperty)
                .HasConversion(
                    i =>
                        i == null ? String.Empty : this._encryption.Encrypt(i),
                    o => this._encryption.Decrypt(o)
                );
        }

        builder
            .Property(u => u.Otp)
            .HasMaxLength(Constants.UserOtpMaxLength);

        builder
            .HasIndex(u => u.EmailAddressNormalized)
            .IsUnique();

        builder
            .HasIndex(u => u.UserNameNormalized)
            .IsUnique();
    }
}
