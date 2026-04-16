using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shipstone.Extensions.Security;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Configuration;

internal sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    private readonly IEncryptionService _encryption;

    internal UserConfiguration(IEncryptionService encryption) =>
        this._encryption = encryption;

    void IEntityTypeConfiguration<UserEntity>.Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder
            .Property(u => u.UserName)
            .HasConversion(
                i =>
                    i == null ? String.Empty : this._encryption.Encrypt(i),
                o => this._encryption.Decrypt(o)
            );

        builder
            .HasIndex(u => u.IdentityId)
            .IsUnique();

        builder
            .HasIndex(u => u.UserNameNormalized)
            .IsUnique();

        builder
            .HasMany<PostEntity>()
            .WithOne()
            .HasForeignKey(p => p.CreatorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany<UserDeviceEntity>()
            .WithOne()
            .HasForeignKey(ud => ud.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany<UserFollowingEntity>()
            .WithOne()
            .HasForeignKey(uf => uf.FolloweeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany<UserFollowingEntity>()
            .WithOne()
            .HasForeignKey(uf => uf.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany<UserRoleEntity>()
            .WithOne()
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
