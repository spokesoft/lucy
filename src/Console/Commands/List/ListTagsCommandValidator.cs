using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Validator for the <see cref="ListTagsCommand"/> command.
/// </summary>
internal class ListTagsCommandValidator(
    IMediator mediator) : ICommandValidator<ListTagsCommand>
{
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ListTagsCommand command,
        CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(command.Key) && !command.Id.HasValue)
        {
            return ValidationResult.Error(ConsoleValidationCode.ProjectKeyOrIdRequired);
        }

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
