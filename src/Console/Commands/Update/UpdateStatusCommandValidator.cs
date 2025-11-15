using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Statuses.Queries.StatusExistsByKey;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Validator for <see cref="UpdateStatusCommand"/>
/// </summary>
public class UpdateStatusCommandValidator(
    IMediator mediator) : ICommandValidator<UpdateStatusCommand>
{
    /// <summary>
    /// The mediator instance.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        UpdateStatusCommand command,
        CancellationToken token = default)
    {
        if (command.StatusId is null)
        {
            // Handle ambiguous case: if StatusKey is null but ProjectKey has a value and ProjectId is set,
            // then ProjectKey is actually the status key (single positional argument with --project-id option)
            string? statusKey = command.StatusKey;
            string? projectKey = command.ProjectKey;
            long? projectId = command.ProjectId;

            if (statusKey is null && projectKey is not null && projectId.HasValue)
            {
                // Single positional argument with --project-id: treat as status key
                statusKey = projectKey;
                projectKey = null;
            }

            if (string.IsNullOrWhiteSpace(statusKey))
                return ValidationResult.Error(ConsoleValidationCode.StatusKeyOrIdRequired);

            if (projectId is null && string.IsNullOrWhiteSpace(projectKey))
                return ValidationResult.Error(ConsoleValidationCode.ProjectKeyOrIdRequiredForStatusKey);

            if (projectId is null)
            {
                var projectExistsQuery = new ProjectExistsByKeyQuery(projectKey!);
                if (!await _mediator.Send(projectExistsQuery, token))
                {
                    return ValidationResult.Error(
                        ConsoleValidationCode.ProjectKeyNotFound,
                        nameof(command.ProjectKey),
                        projectKey!);
                }

                var projectIdQuery = new GetProjectIdByKeyQuery(projectKey!);
                projectId = await _mediator.Send(projectIdQuery, token);
            }

            var statusExistsQuery = new StatusExistsByKeyQuery(projectId.Value, statusKey);
            if (!await _mediator.Send(statusExistsQuery, token))
            {
                return ValidationResult.Error(
                    ConsoleValidationCode.StatusKeyNotFound,
                    nameof(command.StatusKey),
                    statusKey);
            }
        }

        return ValidationResult.Success;
    }
}
