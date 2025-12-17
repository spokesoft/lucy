using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Common.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Validator for the <see cref="ShowBoardCommand"/> command.
/// </summary>
internal class ShowBoardCommandValidator(
    IMediator mediator) : ICommandValidator<ShowBoardCommand>
{
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ShowBoardCommand command,
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
