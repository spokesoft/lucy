using System.Diagnostics;
using Lucy.Application.Interfaces;
using Lucy.Console.Enums;
using Lucy.Console.Extensions;
using Lucy.Console.Interfaces;
using Lucy.Console.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Lucy.Console.Middleware;

/// <summary>
/// A middleware that applies database migrations before executing commands.
/// </summary>
internal class MigrationsMiddleware(
    IServiceProvider services,
    ILogger<MigrationsMiddleware> logger,
    IStringLocalizer<Program> localizer) : ICommandMiddleware
{
    private readonly IServiceProvider _services = services;
    private readonly ILogger<MigrationsMiddleware> _logger = logger;
    private readonly IStringLocalizer<Program> _localizer = localizer;

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
            _logger.LogDebug(_localizer, "Messages.NoMigratorsRegistered");
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
                        _localizer,
                        "Messages.NoMigrationRequired",
                        migrator.Name);
                }
            }

            if (required.Count > 0)
            {
                await Task.WhenAll(required.Select(
                    migrator => MigrateAsync(migrator, token)));

                sw.Stop();
                _logger.LogInformation(
                    _localizer,
                    "Messages.MigrationsCompleted",
                    required.Count,
                    sw.ElapsedMilliseconds);
            }
            else
            {
                sw.Stop();
                _logger.LogDebug(_localizer, "Messages.NoMigrationsRequired");
            }
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            _logger.LogWarning(
                _localizer,
                "Messages.MigrationsCanceled",
                sw.ElapsedMilliseconds);

            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(
                ex,
                _localizer,
                "Messages.MigrationsFailed",
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
                _localizer,
                "Messages.MigrationCompleted",
                migrator.Name,
                sw.ElapsedMilliseconds);
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            _logger.LogError(
                _localizer,
                "Messages.MigrationCanceled",
                migrator.Name,
                sw.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(
                ex,
                _localizer,
                "Messages.MigrationFailed",
                migrator.Name,
                sw.ElapsedMilliseconds);
            throw;
        }
    }
}
