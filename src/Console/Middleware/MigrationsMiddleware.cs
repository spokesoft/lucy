using System.Diagnostics;
using Lucy.Application.Interfaces;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Lucy.Console.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Lucy.Console.Middleware;

/// <summary>
/// A middleware that applies database migrations before executing commands.
/// </summary>
public class MigrationsMiddleware(
    IServiceProvider services,
    ILogger<MigrationsMiddleware> logger) : ICommandMiddleware
{
    private readonly IServiceProvider _services = services;
    private readonly ILogger<MigrationsMiddleware> _logger = logger;

    /// <inheritdoc/>
    public async Task<ExitCode> InvokeAsync<TCommand>(
        CommandContext context,
        TCommand command,
        CommandDelegate<TCommand> next,
        CancellationToken token = default)
        where TCommand : CommandSettings
    {
        var migrators = _services.GetServices<IDatabaseMigrator>();

        if (!migrators.Any())
        {
            _logger.LogDebug("No database migrators registered, skipping migrations.");
            return await next(context, command, token);
        }

        var sw = Stopwatch.StartNew();

        try
        {
            var required = new List<IDatabaseMigrator>();

            foreach (var migrator in migrators)
            {
                if (await migrator.IsMigrationRequiredAsync(token))
                {
                    required.Add(migrator);
                }
                else
                {
                    _logger.LogDebug(
                        "No migration required for {Database}, the database is up to date.",
                        migrator.Name);
                }
            }

            if (required.Count > 0)
            {
                await Task.WhenAll(required.Select(migrator => MigrateAsync(migrator, token)));

                sw.Stop();
                _logger.LogInformation(
                    "Database migrations completed successfully for {Databases} in {Elapsed}ms",
                    required.Count == 1
                        ? "1 database"
                        : $"{required.Count} databases",
                    sw.ElapsedMilliseconds);
            }
            else
            {
                sw.Stop();
                _logger.LogDebug("No database migrations required, all databases are up to date.");
            }
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            _logger.LogWarning(
                "Database migrations canceled after {Elapsed}ms",
                sw.ElapsedMilliseconds);

            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(
                ex,
                "Database migrations failed after {Elapsed}ms",
                sw.ElapsedMilliseconds);

            throw;
        }

        return await next(context, command, token);
    }

    /// <summary>
    /// Migrates a database using the specified migrator.
    /// </summary>
    private async Task MigrateAsync(
        IDatabaseMigrator migrator,
        CancellationToken token = default)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await migrator.MigrateAsync(token);
            sw.Stop();
            _logger.LogInformation(
                "Database migrations for {Database} completed successfully in {Elapsed}ms",
                migrator.Name,
                sw.ElapsedMilliseconds);
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            _logger.LogError(
                "Database migrations for {Database} canceled after {Elapsed}ms",
                migrator.Name,
                sw.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(
                ex,
                "Database migrations for {Database} failed after {Elapsed}ms",
                migrator.Name,
                sw.ElapsedMilliseconds);
            throw;
        }
    }
}
