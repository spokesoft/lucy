using Microsoft.Extensions.Logging;

namespace Lucy.Infrastructure.Logging.Database;

/// <summary>
/// Represents a log entry to be stored in the database.
/// </summary>
public class LogEntry
{
    /// <summary>
    /// The unique identifier for the log entry.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The event ID associated with the log entry.
    /// </summary>
    public EventId EventId { get; set; }

    /// <summary>
    /// The log level of the entry.
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// The category of the log entry.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// The log message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The exception details, if any.
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// The timestamp of the log entry.
    /// </summary>
    public DateTime Timestamp { get; set; }
}
