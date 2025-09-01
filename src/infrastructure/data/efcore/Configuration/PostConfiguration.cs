using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Configuration;

internal readonly struct PostConfiguration
    : IEntityTypeConfiguration<PostEntity>
{
    void IEntityTypeConfiguration<PostEntity>.Configure(EntityTypeBuilder<PostEntity> builder)
    {
        builder
            .Property(p => p.Body)
            .HasMaxLength(Constants.PostBodyMaxLength);

        builder
            .HasMany<PostEntity>()
            .WithOne()
            .HasForeignKey(p => p.ParentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
