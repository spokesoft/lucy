using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Validator for <see cref="DeleteProjectCommand"/>
/// </summary>
public class DeleteProjectCommandValidator : ICommandValidator<DeleteProjectCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(
        CommandContext context,
        DeleteProjectCommand settings,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
