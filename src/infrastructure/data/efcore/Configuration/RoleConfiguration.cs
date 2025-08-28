using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Configuration;

internal readonly struct RoleConfiguration
    : IEntityTypeConfiguration<RoleEntity>
{
    void IEntityTypeConfiguration<RoleEntity>.Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder
            .Property(r => r.Name)
            .HasMaxLength(Constants.RoleNameMaxLength);

        builder
            .Property(r => r.NameNormalized)
            .HasMaxLength(Constants.RoleNameMaxLength);

        builder
            .HasIndex(r => r.NameNormalized)
            .IsUnique();

        builder
            .HasMany<UserRoleEntity>()
            .WithOne()
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
