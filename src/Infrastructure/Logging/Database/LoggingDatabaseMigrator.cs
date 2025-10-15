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

    /// <inheritdoc />
    public string Name => "Logging";

    /// <inheritdoc />
    public Task MigrateAsync(CancellationToken token = default)
        => _context.Database.MigrateAsync(token);

    /// <inheritdoc />
    public Task<bool> IsMigrationRequiredAsync(CancellationToken token = default)
        => _context.Database.GetPendingMigrationsAsync(token).ContinueWith(t => t.Result.Any(), token);
}
