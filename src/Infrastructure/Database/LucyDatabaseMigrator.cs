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

    /// <inheritdoc />
    public string Name => "Lucy";

    /// <inheritdoc />
    public Task MigrateAsync(CancellationToken token = default)
        => _context.Database.MigrateAsync(token);

    /// <inheritdoc />
    public Task<bool> IsMigrationRequiredAsync(CancellationToken token = default)
        => _context.Database.GetPendingMigrationsAsync(token).ContinueWith(t => t.Result.Any(), token);
}
