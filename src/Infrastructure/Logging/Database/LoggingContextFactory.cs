using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lucy.Infrastructure.Logging.Database;

/// <summary>
/// Factory for creating LoggingContext instances (at design time).
/// </summary>
/// <remarks>
/// This is used by EF tools for creating migrations. This connection string
/// does not need to match the runtime configuration. It is only used to
/// create a context for design-time operations.
/// </remarks>
public class LoggingContextFactory : IDesignTimeDbContextFactory<LoggingDbContext>
{
    public LoggingDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<LoggingDbContext>();
        builder.UseSqlite("Data Source=logs.db; Mode=ReadWriteCreate;");

        return new LoggingDbContext(builder.Options);
    }
}
