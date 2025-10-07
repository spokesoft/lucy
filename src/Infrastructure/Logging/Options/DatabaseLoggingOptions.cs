using Lucy.Infrastructure.Database;

namespace Lucy.Infrastructure.Logging.Options;

/// <summary>
/// Configuration options for database logging.
/// </summary>
public sealed class DatabaseLoggingOptions : DatabaseOptionsBase
{
    /// <summary>
    /// The configuration section name for database logging options.
    /// </summary>
    public const string SectionName = "Database";

    /// <summary>
    /// The data source (file path) for the SQLite database.
    /// </summary>
    public string DataSource { get; set; } = "logs.db";

    /// <summary>
    /// The timeout (in seconds) to wait for the logging service to stop gracefully.
    /// </summary>
    public int StopTimeout { get; set; } = 5;
}
