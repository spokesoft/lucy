using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Validator for the <see cref="ListTicketsCommand"/> command.
/// </summary>
internal class ListTicketsCommandValidator(
    IMediator mediator) : ICommandValidator<ListTicketsCommand>
{
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ListTicketsCommand command,
        CancellationToken token = default)
    {
        // Require either key or ID
        if (string.IsNullOrWhiteSpace(command.Key) && !command.Id.HasValue)
        {
            return ValidationResult.Error(ConsoleValidationCode.ProjectKeyOrIdRequired);
        }

        // Validate project exists by key
        if (!string.IsNullOrWhiteSpace(command.Key) && !command.Id.HasValue)
        {
            var query = new ProjectExistsByKeyQuery(command.Key);
            if (!await _mediator.Send(query, token))
            {
                return ValidationResult.Error(
                    ConsoleValidationCode.ProjectKeyNotFound,
                    nameof(command.Key),
                    command.Key);
            }
        }

        return ValidationResult.Success;
    }
}
