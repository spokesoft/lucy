using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Lucy.Console.Extensions;

/// <summary>
/// Extension methods for <see cref="ILogger"/> to support localized logging.
/// </summary>
public static class LoggingExtensions
{
    private static void LogLocalized(
        ILogger logger,
        LogLevel level,
        EventId eventId,
        Exception? exception,
        IStringLocalizer localizer,
        string resourceKey,
        object[] args)
    {
        if (!logger.IsEnabled(level))
            return;

        string message = localizer.GetString(resourceKey);
        logger.Log(level, eventId, exception, message, args);
    }

    // Trace
    public static void LogTrace(this ILogger logger, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Trace, default, null, localizer, key, args);

    public static void LogTrace(this ILogger logger, EventId eventId, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Trace, eventId, null, localizer, key, args);

    // Debug
    public static void LogDebug(this ILogger logger, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Debug, default, null, localizer, key, args);

    public static void LogDebug(this ILogger logger, EventId eventId, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Debug, eventId, null, localizer, key, args);

    // Information
    public static void LogInformation(this ILogger logger, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Information, default, null, localizer, key, args);

    public static void LogInformation(this ILogger logger, EventId eventId, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Information, eventId, null, localizer, key, args);

    // Warning
    public static void LogWarning(this ILogger logger, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Warning, default, null, localizer, key, args);

    public static void LogWarning(this ILogger logger, EventId eventId, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Warning, eventId, null, localizer, key, args);

    public static void LogWarning(this ILogger logger, Exception exception, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Warning, default, exception, localizer, key, args);

    public static void LogWarning(this ILogger logger, EventId eventId, Exception exception, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Warning, eventId, exception, localizer, key, args);

    // Error
    public static void LogError(this ILogger logger, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Error, default, null, localizer, key, args);

    public static void LogError(this ILogger logger, EventId eventId, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Error, eventId, null, localizer, key, args);

    public static void LogError(this ILogger logger, Exception exception, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Error, default, exception, localizer, key, args);

    public static void LogError(this ILogger logger, EventId eventId, Exception exception, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Error, eventId, exception, localizer, key, args);

    // Critical
    public static void LogCritical(this ILogger logger, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Critical, default, null, localizer, key, args);

    public static void LogCritical(this ILogger logger, EventId eventId, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Critical, eventId, null, localizer, key, args);

    public static void LogCritical(this ILogger logger, Exception exception, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Critical, default, exception, localizer, key, args);

    public static void LogCritical(this ILogger logger, EventId eventId, Exception exception, IStringLocalizer localizer, string key, params object[] args)
        => LogLocalized(logger, LogLevel.Critical, eventId, exception, localizer, key, args);
}
