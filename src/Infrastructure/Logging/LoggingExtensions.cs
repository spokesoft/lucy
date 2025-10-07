using System.Threading.Channels;
using Lucy.Application.Interfaces;
using Lucy.Infrastructure.Logging.Database;
using Lucy.Infrastructure.Logging.Options;
using Lucy.Infrastructure.Logging.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lucy.Infrastructure.Logging;

/// <summary>
/// Extension methods for setting up logging services.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Adds logging services to the service collection.
    /// </summary>
    public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(LoggingOptions.SectionName);
        var options = new LoggingOptions();

        section.Bind(options);

        services.Configure<DatabaseLoggingOptions>(dlo =>
            section.GetSection(DatabaseLoggingOptions.SectionName).Bind(dlo));

        services.AddLogging(builder =>
        {
            builder.AddConfiguration(section);
            builder.SetMinimumLevel(options.MinimumLogLevel);
            builder.AddDatabase(options);
        });

        return services;
    }

    /// <summary>
    /// Adds a database logger to the logging builder.
    /// </summary>
    public static ILoggingBuilder AddDatabase(this ILoggingBuilder builder, LoggingOptions options)
    {
        var channelOptions = new BoundedChannelOptions(options.Channel.Capacity)
        {
            FullMode = options.Channel.FullMode,
            SingleReader = true,
            SingleWriter = false
        };

        var channel = Channel.CreateBounded<LogEntry>(channelOptions);
        var provider = new DatabaseLoggerProvider(channel, options);

        builder.Services
            .AddSingleton(channel)
            .AddDbContext<LoggingDbContext>(config
                => config.UseSqlite(BuildConnectionString(options.Database, SqliteOpenMode.ReadWriteCreate)))
            .AddDbContext<LoggingReadContext>(config
                => config.UseSqlite(BuildConnectionString(options.Database, SqliteOpenMode.ReadOnly)))
            .AddDbContext<LoggingWriteContext>(config
                => config.UseSqlite(BuildConnectionString(options.Database, SqliteOpenMode.ReadWrite)))
            .AddSingleton<IDatabaseMigrator, LoggingDatabaseMigrator>()
            .AddSingleton<ILoggerProvider>(provider)
            .AddSingleton<IDatabaseLoggingService, DatabaseLoggingService>();

        return builder;
    }

    /// <summary>
    /// Builds the SQLite connection string based on the provided options and mode.
    /// </summary>
    private static string BuildConnectionString(DatabaseLoggingOptions options, SqliteOpenMode mode)
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = options.DataSource,
            Mode = mode,
            Cache = options.Cache,
            Pooling = options.Pooling,
            ForeignKeys = options.ForeignKeys,
            RecursiveTriggers = options.RecursiveTriggers,
            DefaultTimeout = options.DefaultTimeout
        };
        return builder.ToString();
    }
}
