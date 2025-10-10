using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Validator for <see cref="NewProjectCommand"/>
/// </summary>
public class NewProjectCommandValidator : ICommandValidator<NewProjectCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(
        CommandContext context,
        NewProjectCommand settings,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
