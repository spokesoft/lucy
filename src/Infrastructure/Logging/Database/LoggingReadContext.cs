using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Logging.Database;

/// <summary>
/// Database context for reading logs only.
/// </summary>
public class LoggingReadContext(
    DbContextOptions<LoggingDbContext> options) : LoggingDbContext(options)
{
    private const string ReadOnlyErrorMessage = "This context is read-only and does not support saving changes.";

    /// <summary>
    /// Configures the context to be read-only.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    /// <summary>
    /// Prevents saving changes to the database.
    /// </summary>
    public override int SaveChanges()
        => throw new InvalidOperationException(ReadOnlyErrorMessage);

    /// <summary>
    /// Prevents saving changes to the database asynchronously.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken token = default)
        => throw new InvalidOperationException(ReadOnlyErrorMessage);
}
