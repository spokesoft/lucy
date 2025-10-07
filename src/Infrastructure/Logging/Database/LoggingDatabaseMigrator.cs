using Lucy.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Logging.Database;

public class LoggingDatabaseMigrator(
    LoggingDbContext context) : IDatabaseMigrator
{
    /// <summary>
    /// The database context used for migrations.
    /// </summary>
    private readonly LoggingDbContext _context = context;

    /// <summary>
    /// Applies any pending migrations to the database.
    /// </summary>
    public Task MigrateAsync(CancellationToken token = default)
        => _context.Database.MigrateAsync(token);
}
