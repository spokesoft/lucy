using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lucy.Infrastructure.Logging.Options;

/// <summary>
/// Configuration options for logging.
/// </summary>
public class LoggingOptions
{
    public static readonly string SectionName = "Logging";

    [ConfigurationKeyName("LogLevel:Minimum")]
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Trace;

    [ConfigurationKeyName("LogLevel:Default")]
    public LogLevel DefaultLogLevel { get; set; } = LogLevel.Information;

    [ConfigurationKeyName("LogLevel")]
    public Dictionary<string, LogLevel> LogLevels { get; set; } = [];

    [ConfigurationKeyName("Channel")]
    public LoggingChannelOptions Channel { get; set; } = new();

    [ConfigurationKeyName("Database")]
    public DatabaseLoggingOptions Database { get; set; } = new();
}
