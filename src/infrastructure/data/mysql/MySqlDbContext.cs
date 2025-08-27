using Microsoft.EntityFrameworkCore;

using Shipstone.Extensions.Security;

using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;

namespace Shipstone.OpenBook.Api.Infrastructure.Data.MySql;

internal sealed class MySqlDbContext : DbContext<MySqlDbContext>
{
    internal MySqlDbContext(
        DbContextOptions<MySqlDbContext> options,
        IEncryptionService encryption
    )
        : base(options, encryption)
    { }
}
