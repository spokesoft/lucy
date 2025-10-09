using Lucy.Application.Validation;
using Spectre.Console.Cli;

namespace Lucy.Console.Interfaces;

/// <summary>
/// Interface for command validators.
/// </summary>
public interface ICommandValidator<TCommand>
    where TCommand : CommandSettings
{
    /// <summary>
    /// Validates the command settings asynchronously.
    /// </summary>
    Task<ValidationResult> ValidateAsync(CommandContext context, TCommand settings, CancellationToken token = default);
}
