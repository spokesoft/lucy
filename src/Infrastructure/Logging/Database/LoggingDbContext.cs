using Lucy.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Logging.Database;

/// <summary>
/// Database context for logging.
/// </summary>
public class LoggingDbContext(
    DbContextOptions<LoggingDbContext> options) : DbContext(options)
{
    /// <summary>
    /// The namespace where the entity configurations are located.
    /// </summary>
    private const string ConfigurationNamespace = "Lucy.Infrastructure.Logging.Database.Configurations";

    /// <summary>
    /// The log entries in the database.
    /// </summary>
    public DbSet<LogEntry> Logs { get; set; } = null!;

    /// <summary>
    /// Configures the model for the logging context.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromNamespace(
            typeof(LoggingDbContext).Assembly,
            ConfigurationNamespace);

        base.OnModelCreating(builder);
    }
}
