using Lucy.Application.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lucy.Infrastructure.Database;

/// <summary>
/// Extension methods for configuring database services.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Adds the database contexts to the service collection using the provided configuration.
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(DatabaseOptions.SectionName);
        var options = new DatabaseOptions();

        section.Bind(options);

        services
            .AddDbContext<LucyDbContext>(config
                => config.UseSqlite(BuildConnectionString(options, SqliteOpenMode.ReadWriteCreate)))
            .AddDbContext<LucyReadContext>(config
                => config.UseSqlite(BuildConnectionString(options, SqliteOpenMode.ReadOnly)))
            .AddDbContext<LucyWriteContext>(config
                => config.UseSqlite(BuildConnectionString(options, SqliteOpenMode.ReadWrite)))
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IReadOnlyUnitOfWork, ReadOnlyUnitOfWork>()
            .AddSingleton<IDatabaseMigrator, LucyDatabaseMigrator>();

        return services;
    }

    /// <summary>
    /// Builds a SQLite connection string with the specified options and open mode.
    /// </summary>
    public static string BuildConnectionString(DatabaseOptions options, SqliteOpenMode mode)
        => new SqliteConnectionStringBuilder
        {
            DataSource = options.DataSource,
            Mode = mode,
            Cache = SqliteCacheMode.Shared,
            Pooling = options.Pooling,
            ForeignKeys = options.ForeignKeys,
            RecursiveTriggers = options.RecursiveTriggers,
            DefaultTimeout = options.DefaultTimeout,
        }.ToString();
}
