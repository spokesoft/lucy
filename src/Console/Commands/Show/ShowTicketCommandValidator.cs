using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Validator for <see cref="ShowTicketCommand"/>
/// </summary>
internal class ShowTicketCommandValidator : ICommandValidator<ShowTicketCommand>
{
    /// <inheritdoc />
    public Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ShowTicketCommand command,
        CancellationToken token = default)
    {
        // No validation needed since ticket ID is required
        return Task.FromResult(ValidationResult.Success);
    }
}
