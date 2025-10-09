using Lucy.Application.Interfaces;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Lucy.Infrastructure.Logging.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Internal;

/// <summary>
/// A factory for creating command pipelines.
/// </summary>
internal class CommandPipeline
{
    /// <summary>
    /// Creates a new command pipeline for the specified command and settings.
    /// </summary>
    public static CommandPipeline<TCommand> Create<TCommand>(CommandContext context, TCommand settings)
        where TCommand : CommandSettings
    {
        return new CommandPipeline<TCommand>(context, settings);
    }
}

/// <summary>
/// A pipeline to process command execution steps.
/// </summary>
internal class CommandPipeline<TCommand>(CommandContext context, TCommand settings)
    where TCommand : CommandSettings
{
    /// <summary>
    /// The command context.
    /// </summary>
    private readonly CommandContext _context = context;

    /// <summary>
    /// The command settings.
    /// </summary>
    private readonly TCommand _settings = settings;

    /// <summary>
    /// The steps to execute in the pipeline.
    /// </summary>
    private readonly List<Func<CancellationToken, Func<CancellationToken, Task<ExitCode>>, Task<ExitCode>>> _steps = [];

    /// <summary>
    /// Adds a database migration step to the pipeline.
    /// </summary>
    public CommandPipeline<TCommand> UseDatabaseMigration(IEnumerable<IDatabaseMigrator> migrators)
    {
        if (!migrators.Any())
            return this;

        _steps.Add(async (token, next) =>
        {
            await Task.WhenAll(migrators.Select(migrator => migrator.MigrateAsync(token)));
            return await next(token);
        });

        return this;
    }

    /// <summary>
    /// Adds a validation step to the pipeline.
    /// </summary>
    public CommandPipeline<TCommand> UseValidation(IEnumerable<ICommandValidator<TCommand>> validators)
    {
        if (!validators.Any())
            return this;

        _steps.Add(async (token, next) =>
        {
            var exitCode = ExitCode.Success;
            var result = new Application.Validation.ValidationResult();
            var validations = validators.Select(async validator =>
            {
                result.AddResult(
                    await validator.ValidateAsync(_context, _settings, token));
            });

            await Task.WhenAll(validations);

            if (result.IsValid)
            {
                try
                {
                    exitCode = await next(token);
                }
                // Catch validation exceptions thrown during command
                // handling. These exceptions are expected to be thrown by
                // the application layer.
                catch (ValidationException ex)
                {
                    result.AddResult(ex.Result);
                }
            }

            if (!result.IsValid)
            {
                // TODO: Handle the validation result.
                return ExitCode.Invalid;
            }

            return exitCode;
        });

        return this;
    }

    /// <summary>
    /// Adds a logging step to the pipeline.
    /// </summary>
    public CommandPipeline<TCommand> UseLogging(IDatabaseLoggingService logging)
    {
        _steps.Add(async (token, next) =>
        {
            logging.Start(token);
            try
            {
                return await next(token);
            }
            finally
            {
                await logging.StopAsync();
            }
        });

        return this;
    }

    /// <summary>
    /// Adds a command handler execution step to the pipeline.
    /// </summary>
    public CommandPipeline<TCommand> UseHandler(ICommandHandler<TCommand> handler)
    {
        _steps.Add((token, next) => handler.HandleAsync(_context, _settings, token));
        return this;
    }

    /// <summary>
    /// Runs the command pipeline asynchronously.
    /// </summary>
    public async Task<int> RunAsync()
    {
        using var cts = new CancellationTokenSource();

        Func<CancellationToken, Task<ExitCode>> final = ct
            => Task.FromResult(ExitCode.Success);

        // Compose the steps in reverse order so that each step calls the next step.
        foreach (var step in _steps.AsEnumerable().Reverse())
        {
            var next = final;
            final = token => step(token, next);
        }

        ExitCode exitCode = await final(cts.Token);
        return (int)exitCode;
    }
}
