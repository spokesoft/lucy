using System.Threading.Channels;
using Lucy.Infrastructure.Logging.Database;
using Microsoft.Extensions.Logging;

namespace Lucy.Infrastructure.Logging;

/// <summary>
/// A logger that writes log entries to a channel for database logging.
/// </summary>
public class DatabaseLogger(
    string name,
    ChannelWriter<LogEntry> writer,
    LogLevel minLevel = LogLevel.Trace) : ILogger
{
    /// <summary>
    /// The minimum log level for this logger.
    /// </summary>
    private readonly LogLevel _minLevel = minLevel;

    /// <summary>
    /// The category name for this logger.
    /// </summary>
    private readonly string _name = name;

    /// <summary>
    /// The channel writer to write log entries to.
    /// </summary>
    private readonly ChannelWriter<LogEntry> _writer = writer;

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => null;

    /// <summary>
    /// Checks if the given log level is enabled.
    /// </summary>
    public bool IsEnabled(LogLevel level) => level >= _minLevel;

    /// <summary>
    /// Logs a message with the specified log level and details.
    /// </summary>
    public void Log<TState>(LogLevel level, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(level))
            return;

        var message = formatter?.Invoke(state, exception)
            ?? state?.ToString()
            ?? string.Empty;

        var entry = new LogEntry
        {
            EventId = eventId,
            Level = level,
            Category = _name,
            Message = message,
            Exception = exception?.ToString(),
            Timestamp = DateTime.UtcNow
        };

        _writer.TryWrite(entry);
    }
}
