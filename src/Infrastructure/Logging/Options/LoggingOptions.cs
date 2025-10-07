using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lucy.Infrastructure.Logging.Options;

/// <summary>
/// Configuration options for logging.
/// </summary>
public sealed class LoggingOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public static readonly string SectionName = "Logging";

    /// <summary>
    /// The minimum log level.
    /// </summary>
    [ConfigurationKeyName("LogLevel:Minimum")]
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Trace;

    /// <summary>
    /// The default log level.
    /// </summary>
    [ConfigurationKeyName("LogLevel:Default")]
    public LogLevel DefaultLogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// The log levels for specific categories.
    /// </summary>
    [ConfigurationKeyName("LogLevel")]
    public Dictionary<string, LogLevel> LogLevels { get; set; } = [];

    /// <summary>
    /// The logging channel options.
    /// </summary>
    public LoggingChannelOptions Channel { get; set; } = new();

    /// <summary>
    /// The database logging options.
    /// </summary>
    public DatabaseLoggingOptions Database { get; set; } = new();
}
