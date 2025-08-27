using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.Extensions.Security;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Configuration;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;

/// <summary>
/// Represents a database.
/// </summary>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public abstract class DbContext<TContext> : DbContext, IDataSource
    where TContext : DbContext<TContext>
{
    private readonly IEncryptionService _encryption;
    private readonly DbSet<UserEntity> _users;

    public DbSet<UserEntity> Users => this._users;

    IDataSet<UserEntity> IDataSource.Users =>
        new DataSet<UserEntity>(this._users);

    protected DbContext(
        DbContextOptions<TContext> options,
        IEncryptionService encryption
    ) : base(options)
    {
        ArgumentNullException.ThrowIfNull(encryption);
        this._encryption = encryption;
        this._users = this.Set<UserEntity>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        IEntityTypeConfiguration<UserEntity> userConfiguration =
            new UserConfiguration(this._encryption);

        modelBuilder.ApplyConfiguration(userConfiguration);
    }

    Task IDataSource.SaveAsync(CancellationToken cancellationToken) =>
        this.SaveChangesAsync(cancellationToken);
}
