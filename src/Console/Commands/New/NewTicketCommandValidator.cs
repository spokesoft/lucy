using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectByKey;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Application.Common.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Validator for the <see cref="NewTicketCommand"/> command.
/// </summary>
internal class NewTicketCommandValidator(
    IMediator mediator) : ICommandValidator<NewTicketCommand>
{
    /// <summary>
    /// The mediator instance for sending queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        NewTicketCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Validate that either ProjectKey or ProjectId is provided
        if (command.ProjectKey is null && command.ProjectId is null)
        {
            result.AddError(
                ConsoleValidationCode.ProjectKeyOrIdRequired,
                nameof(command.ProjectKey));
            return result;
        }

        // Resolve and validate ProjectId
        long? projectId = command.ProjectId;
        if (projectId is null && command.ProjectKey is not null)
        {
            var projectQuery = new GetProjectByKeyQuery(command.ProjectKey);
            var project = await _mediator.Send(projectQuery, token);

            if (project is null)
            {
                result.AddError(
                    ConsoleValidationCode.ProjectKeyNotFound,
                    nameof(command.ProjectKey),
                    command.ProjectKey);
                return result;
            }

            projectId = project.Id;
        }

        // Validate StatusKey if provided
        if (command.StatusKey is not null && projectId.HasValue)
        {
            var statusQuery = new GetStatusByKeyQuery(projectId.Value, command.StatusKey);
            var status = await _mediator.Send(statusQuery, token);

            if (status is null)
            {
                result.AddError(new ValidationError(
                    $"Status with key '{command.StatusKey}' not found in project.",
                    nameof(command.StatusKey)));
            }
        }

        return result;
    }
}
