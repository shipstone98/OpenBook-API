using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.Extensions.Security;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore.Configuration;
using Shipstone.OpenBook.Api.Infrastructure.Entities;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;

/// <summary>
/// Represents a database. This class cannot be instantiated.
/// </summary>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public abstract class DbContext<TContext> : DbContext, IDataSource
    where TContext : DbContext<TContext>
{
    private readonly IEncryptionService _encryption;
    private readonly DbSet<PostEntity> _posts;
    private readonly DbSet<RoleEntity> _roles;
    private readonly DbSet<UserRefreshTokenEntity> _userRefreshTokens;
    private readonly DbSet<UserRoleEntity> _userRoles;
    private readonly DbSet<UserEntity> _users;

    public DbSet<PostEntity> Posts => this._posts;
    public DbSet<RoleEntity> Roles => this._roles;

    public DbSet<UserRefreshTokenEntity> UserRefreshTokens =>
        this._userRefreshTokens;

    public DbSet<UserRoleEntity> UserRoles => this._userRoles;
    public DbSet<UserEntity> Users => this._users;

    IDataSet<PostEntity> IDataSource.Posts =>
        new DataSet<PostEntity>(this._posts);

    IDataSet<RoleEntity> IDataSource.Roles =>
        new DataSet<RoleEntity>(this._roles);

    IDataSet<UserRefreshTokenEntity> IDataSource.UserRefreshTokens =>
        new DataSet<UserRefreshTokenEntity>(this._userRefreshTokens);

    IDataSet<UserRoleEntity> IDataSource.UserRoles =>
        new DataSet<UserRoleEntity>(this._userRoles);

    IDataSet<UserEntity> IDataSource.Users =>
        new DataSet<UserEntity>(this._users);

    protected DbContext(
        DbContextOptions<TContext> options,
        IEncryptionService encryption
    ) : base(options)
    {
        ArgumentNullException.ThrowIfNull(encryption);
        this._encryption = encryption;
        this._posts = this.Set<PostEntity>();
        this._roles = this.Set<RoleEntity>();
        this._userRefreshTokens = this.Set<UserRefreshTokenEntity>();
        this._userRoles = this.Set<UserRoleEntity>();
        this._users = this.Set<UserEntity>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        IEntityTypeConfiguration<PostEntity> postConfiguration =
            new PostConfiguration();

        IEntityTypeConfiguration<RoleEntity> roleConfiguration =
            new RoleConfiguration();

        IEntityTypeConfiguration<UserEntity> userConfiguration =
            new UserConfiguration(this._encryption);

        IEntityTypeConfiguration<UserRoleEntity> userRoleConfiguration =
            new UserRoleConfiguration();

        modelBuilder
            .ApplyConfiguration(postConfiguration)
            .ApplyConfiguration(roleConfiguration)
            .ApplyConfiguration(userConfiguration)
            .ApplyConfiguration(userRoleConfiguration);
    }

    Task IDataSource.SaveAsync(CancellationToken cancellationToken) =>
        this.SaveChangesAsync(cancellationToken);
}
