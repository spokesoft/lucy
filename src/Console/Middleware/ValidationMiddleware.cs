using System.Diagnostics;
using Lucy.Application.Validation;
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
/// A middleware that handles command validation.
/// </summary>
internal class ValidationMiddleware(
    IServiceProvider services,
    ILogger<ValidationMiddleware> logger,
    IStringLocalizer<Program> localizer) : ICommandMiddleware
{
    private readonly IServiceProvider _services = services;
    private readonly ILogger<ValidationMiddleware> _logger = logger;
    private readonly IStringLocalizer<Program> _localizer = localizer;

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
            _logger.LogDebug(
                _localizer,
                "Messages.NoValidatorsFound",
                commandName);
        }
        else
        {
            try
            {
                foreach (var validator in validators)
                {
                    result.AddResult(await ValidateAsync(validator, context, command, token));
                }

                sw.Stop();
                if (result.IsValid)
                {
                    var count = validators.Count();

                    _logger.LogDebug(
                        _localizer,
                        "Messages.CommandValidationPassed",
                        commandName,
                        count,
                        sw.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogWarning(
                        _localizer,
                        "Messages.CommandValidationFailed",
                        commandName,
                        sw.ElapsedMilliseconds,
                        string.Join("; ", result.Errors.Select(e => e.Message)));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    _localizer,
                    "Messages.CommandValidationCanceled",
                    commandName);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    _localizer,
                    "Messages.CommandValidationError",
                    commandName);
                throw;
            }

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
            if (result.IsValid)
            {
                _logger.LogDebug(
                    _localizer,
                    "Messages.CommandValidatorPassed",
                    commandName,
                    validatorName,
                    sw.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogWarning(
                    _localizer,
                    "Messages.CommandValidatorFailed",
                    commandName,
                    validatorName,
                    sw.ElapsedMilliseconds,
                    string.Join("; ", result.Errors.Select(e => e.Message)));
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                _localizer,
                "Messages.CommandValidatorCanceled",
                commandName,
                validatorName,
                sw.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                _localizer,
                "Messages.CommandValidatorError",
                commandName,
                validatorName,
                sw.ElapsedMilliseconds);
            throw;
        }
    }
}
