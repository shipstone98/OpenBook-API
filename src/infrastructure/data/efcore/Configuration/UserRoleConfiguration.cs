using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Configuration;

internal readonly struct UserRoleConfiguration
    : IEntityTypeConfiguration<UserRoleEntity>
{
    void IEntityTypeConfiguration<UserRoleEntity>.Configure(EntityTypeBuilder<UserRoleEntity> builder) =>
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });
}
