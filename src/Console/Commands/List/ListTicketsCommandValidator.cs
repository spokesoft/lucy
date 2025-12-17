using Lucy.Application.Common.Interfaces;
using Lucy.Application.Iterations.Queries.IterationExistsByKey;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Common.Validation;
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
        bool hasProject = !string.IsNullOrWhiteSpace(command.Key) || command.Id.HasValue;
        bool hasIteration = !string.IsNullOrWhiteSpace(command.IterationKey) || command.IterationId.HasValue;

        // Require either project or iteration
        if (!hasProject && !hasIteration)
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

        // Validate iteration exists by key
        if (!string.IsNullOrWhiteSpace(command.IterationKey) && !command.IterationId.HasValue)
        {
            var query = new IterationExistsByKeyQuery(command.IterationKey);
            if (!await _mediator.Send(query, token))
            {
                return ValidationResult.Error(
                    ConsoleValidationCode.IterationKeyNotFound,
                    nameof(command.IterationKey),
                    command.IterationKey);
            }
        }

        return ValidationResult.Success;
    }
}
