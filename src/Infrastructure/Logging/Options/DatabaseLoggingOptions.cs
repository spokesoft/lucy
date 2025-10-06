using Microsoft.Data.Sqlite;

namespace Lucy.Infrastructure.Logging.Options;

/// <summary>
/// Configuration options for database logging.
/// </summary>
public class DatabaseLoggingOptions
{
    /// <summary>
    /// The configuration section name for database options.
    /// </summary>
    public static readonly string SectionName = "Database";

    /// <summary>
    /// The data source (file path) for the SQLite database.
    /// </summary>
    public string DataSource { get; set; } = "logs.db";

    /// <summary>
    /// The SQLite cache mode to use.
    /// </summary>
    public SqliteCacheMode Cache { get; set; } = SqliteCacheMode.Shared;

    /// <summary>
    /// Whether to enable connection pooling.
    /// </summary>
    public bool Pooling { get; set; } = false;

    /// <summary>
    /// Whether to enable foreign key constraints.
    /// </summary>
    public bool ForeignKeys { get; set; } = false;

    /// <summary>
    /// Whether to enable recursive triggers.
    /// </summary>
    public bool RecursiveTriggers { get; set; } = false;

    /// <summary>
    /// The default timeout (in seconds) for database commands.
    /// </summary>
    public int DefaultTimeout { get; set; } = 30;

    /// <summary>
    /// The timeout (in seconds) to wait for the logging service to stop gracefully.
    /// </summary>
    public int StopTimeout { get; set; } = 5;
}
