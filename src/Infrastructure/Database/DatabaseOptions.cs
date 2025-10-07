using Microsoft.Data.Sqlite;

namespace Lucy.Infrastructure.Database;

/// <summary>
/// Configuration options for database logging.
/// </summary>
public sealed class DatabaseOptions : DatabaseOptionsBase
{
    /// <summary>
    /// The configuration section name for database options.
    /// </summary>
    public const string SectionName = "Database";

    /// <summary>
    /// The data source (file path) for the SQLite database.
    /// </summary>
    public string DataSource { get; set; } = "lucy.db";
}

/// <summary>
/// Base class for database configuration options.
/// </summary>
public abstract class DatabaseOptionsBase
{
    /// <summary>
    /// The SQLite cache mode to use.
    /// </summary>
    public SqliteCacheMode Cache { get; set; } = SqliteCacheMode.Shared;

    /// <summary>
    /// Whether to enable connection pooling.
    /// </summary>
    public bool Pooling { get; set; } = true;

    /// <summary>
    /// Whether to enable foreign key constraints.
    /// </summary>
    public bool ForeignKeys { get; set; } = true;

    /// <summary>
    /// Whether to enable recursive triggers.
    /// </summary>
    public bool RecursiveTriggers { get; set; } = true;

    /// <summary>
    /// The default timeout (in seconds) for database commands.
    /// </summary>
    public int DefaultTimeout { get; set; } = 30;
}
