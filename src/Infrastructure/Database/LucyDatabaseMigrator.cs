using Lucy.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Database;

/// <summary>
/// Database migrator for LucyDbContext.
/// </summary>
public class LucyDatabaseMigrator(
    LucyDbContext context) : IDatabaseMigrator
{
    /// <summary>
    /// The database context used for migrations.
    /// </summary>
    private readonly LucyDbContext _context = context;

    /// <summary>
    /// Applies any pending migrations to the database.
    /// </summary>
    public Task MigrateAsync(CancellationToken token = default)
        => _context.Database.MigrateAsync(token);
}
