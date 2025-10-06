using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace Lucy.Infrastructure.Logging.Database.Converters;

/// <summary>
/// Converts LogLevel to and from a string representation.
/// </summary>
public class LogLevelConverter : ValueConverter<LogLevel, string>
{
    public LogLevelConverter() : base(
        v => ToProvider(v),
        v => FromProvider(v))
    {
    }

    /// <summary>
    /// Converts a LogLevel to its string representation.
    /// </summary>
    public static string ToProvider(LogLevel level) => level.ToString();

    /// <summary>
    /// Converts a string back to a LogLevel.
    /// </summary>
    public static LogLevel FromProvider(string value)
    {
        return Enum.TryParse<LogLevel>(value, out var level) ? level : LogLevel.None;
    }
}
