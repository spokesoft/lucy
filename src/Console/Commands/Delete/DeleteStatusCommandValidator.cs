using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Statuses.Queries.StatusExistsByKey;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Common.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Validator for <see cref="DeleteStatusCommand"/>
/// </summary>
public class DeleteStatusCommandValidator(
    IMediator mediator) : ICommandValidator<DeleteStatusCommand>
{
    /// <summary>
    /// The mediator instance.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        DeleteStatusCommand command,
        CancellationToken token = default)
    {
        // Validate project identification
        long? projectId = command.ProjectId;
        if (projectId is null)
        {
            if (string.IsNullOrWhiteSpace(command.ProjectKey))
                return ValidationResult.Error(ConsoleValidationCode.ProjectKeyOrIdRequired);

            var projectExistsQuery = new ProjectExistsByKeyQuery(command.ProjectKey);
            if (!await _mediator.Send(projectExistsQuery, token))
            {
                return ValidationResult.Error(
                    ConsoleValidationCode.ProjectKeyNotFound,
                    nameof(command.ProjectKey),
                    command.ProjectKey);
            }

            var projectIdQuery = new GetProjectIdByKeyQuery(command.ProjectKey);
            projectId = await _mediator.Send(projectIdQuery, token);
        }

        // Validate status identification
        if (command.StatusId is null)
        {
            if (string.IsNullOrWhiteSpace(command.StatusKey))
                return ValidationResult.Error(ConsoleValidationCode.StatusKeyOrIdRequired);

            var statusExistsQuery = new StatusExistsByKeyQuery(projectId!.Value, command.StatusKey);
            if (!await _mediator.Send(statusExistsQuery, token))
            {
                return ValidationResult.Error(
                    ConsoleValidationCode.StatusKeyNotFound,
                    nameof(command.StatusKey),
                    command.StatusKey);
            }
        }

        return ValidationResult.Success;
    }
}
