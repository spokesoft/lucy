using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Logging.Database;

/// <summary>
/// Database context for writing logs.
/// </summary>
public class LoggingWriteContext(
    DbContextOptions<LoggingContext> options) : LoggingContext(options)
{
    // This context allows write operations, so no additional configuration is needed.
}
