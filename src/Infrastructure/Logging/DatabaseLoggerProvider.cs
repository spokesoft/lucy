using System.Collections.Concurrent;
using System.Threading.Channels;
using Lucy.Infrastructure.Logging.Database;
using Lucy.Infrastructure.Logging.Options;
using Microsoft.Extensions.Logging;

namespace Lucy.Infrastructure.Logging;

/// <summary>
/// Provides instances of <see cref="DatabaseLogger"/>.
/// </summary>
public class DatabaseLoggerProvider(
    Channel<LogEntry> channel,
    LoggingOptions options) : ILoggerProvider
{
    /// <summary>
    /// A thread-safe dictionary to hold loggers for different categories.
    /// </summary>
    private readonly ConcurrentDictionary<string, DatabaseLogger> _loggers = [];

    /// <summary>
    /// The mapping of category names to their minimum log levels.
    /// </summary>
    private readonly Dictionary<string, LogLevel> _categoryLevels = options.LogLevels;

    /// <summary>
    /// The channel writer to enqueue log entries.
    /// </summary>
    private readonly ChannelWriter<LogEntry> _writer = channel.Writer;

    /// <summary>
    /// Creates or retrieves a logger for the specified category.
    /// </summary>
    public ILogger CreateLogger(string categoryName)
    {
        var minLevel = _categoryLevels.TryGetValue(categoryName, out var level)
            ? level
            : options.DefaultLogLevel;
        return _loggers.GetOrAdd(categoryName, name => new DatabaseLogger(name, _writer, minLevel));
    }

    /// <summary>
    /// Disposes the logger provider and its resources.
    /// </summary>
    public void Dispose()
    {
        _loggers.Clear();
        GC.SuppressFinalize(this);
    }
}
