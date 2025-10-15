using System.Diagnostics;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Lucy.Console.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Lucy.Console.Middleware;

/// <summary>
/// A middleware that handles command validation.
/// </summary>
public class ValidationMiddleware(
    IServiceProvider services,
    ILogger<ValidationMiddleware> logger) : ICommandMiddleware
{
    private readonly IServiceProvider _services = services;
    private readonly ILogger<ValidationMiddleware> _logger = logger;

    /// <inheritdoc/>
    public async Task<ExitCode> InvokeAsync<TCommand>(
        CommandContext context,
        TCommand command,
        CommandDelegate<TCommand> next,
        CancellationToken token = default)
        where TCommand : CommandSettings
    {
        var commandType = command.GetType();
        var commandName = commandType.Name;
        var validatorType = typeof(ICommandValidator<>).MakeGenericType(commandType);
        var validators = _services.GetServices(validatorType)
            as IEnumerable<ICommandValidator<TCommand>> ?? [];

        var sw = Stopwatch.StartNew();
        var result = new ValidationResult();

        if (!validators.Any())
        {
            sw.Stop();
            _logger.LogDebug("No validators found for command {command}, skipping validation.", commandName);
        }
        else
        {
            foreach (var validator in validators)
            {
                result.AddResult(await ValidateAsync(validator, context, command, token));
            }

            sw.Stop();
            _logger.LogDebug(
                "Validation for command {command} completed in {elapsed}ms",
                commandName,
                sw.ElapsedMilliseconds);
        }

        if (!result.IsValid)
            throw new ValidationException(result);

        return await next(context, command, token);
    }

    /// <summary>
    /// Validates a command using the specified validator.
    /// </summary>
    private async Task<ValidationResult> ValidateAsync<TCommand>(
        ICommandValidator<TCommand> validator,
        CommandContext context,
        TCommand command,
        CancellationToken token)
        where TCommand : CommandSettings
    {
        var commandName = command.GetType().Name;
        var validatorName = validator.GetType().Name;
        var sw = Stopwatch.StartNew();
        try
        {
            var result = await validator.ValidateAsync(context, command, token);

            sw.Stop();
            _logger.LogDebug(
                "Validation for command {command} with {validator} completed in {elapsed}ms",
                commandName,
                validatorName,
                sw.ElapsedMilliseconds);

            return result;
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            _logger.LogWarning("Validation for command {command} with {validator} was canceled after {elapsed}ms",
                commandName,
                validatorName,
                sw.ElapsedMilliseconds);
            throw;
        }
        catch (Exception)
        {
            _logger.LogError("Validation for command {command} with {validator} failed after {elapsed}ms",
                commandName,
                validatorName,
                sw.ElapsedMilliseconds);
            throw;
        }
    }
}
