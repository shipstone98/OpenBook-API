using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Configuration;

internal readonly struct UserFollowingConfiguration
    : IEntityTypeConfiguration<UserFollowingEntity>
{
    void IEntityTypeConfiguration<UserFollowingEntity>.Configure(EntityTypeBuilder<UserFollowingEntity> builder) =>
        builder.HasKey(uf => new { uf.FollowerId, uf.FolloweeId });
}
