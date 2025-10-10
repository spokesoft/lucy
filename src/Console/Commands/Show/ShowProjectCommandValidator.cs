using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Validator for <see cref="ShowProjectCommand"/>
/// </summary>
public class ShowProjectCommandValidator : ICommandValidator<ShowProjectCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ShowProjectCommand settings,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
