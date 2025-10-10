using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Validator for <see cref="UpdateProjectCommand"/>
/// </summary>
public class UpdateProjectCommandValidator : ICommandValidator<UpdateProjectCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(
        CommandContext context,
        UpdateProjectCommand settings,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
