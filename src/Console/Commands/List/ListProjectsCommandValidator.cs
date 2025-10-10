using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Validator for <see cref="ListProjectsCommand"/>
/// </summary>
public class ListProjectsCommandValidator : ICommandValidator<ListProjectsCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ListProjectsCommand settings,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
