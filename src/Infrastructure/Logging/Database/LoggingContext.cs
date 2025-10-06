using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Logging.Database;

/// <summary>
/// Database context for logging.
/// </summary>
public class LoggingContext(
    DbContextOptions<LoggingContext> options) : DbContext(options)
{
    /// <summary>
    /// The log entries in the database.
    /// </summary>
    public DbSet<LogEntry> Logs { get; set; } = null!;

    /// <summary>
    /// Configures the model for the logging context.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new LogEntryTypeConfiguration());
    }
}
