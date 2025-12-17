using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectByKey;
using Lucy.Application.Common.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Validator for <see cref="ShowStatusCommand"/>
/// </summary>
internal class ShowStatusCommandValidator(
    IMediator mediator) : ICommandValidator<ShowStatusCommand>
{
    /// <summary>
    /// The mediator instance for sending queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ShowStatusCommand command,
        CancellationToken token = default)
    {
        // If using status ID, no other validation needed
        if (command.StatusId is not null)
            return ValidationResult.Success;

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

        // If using status key, need project key or ID
        if (string.IsNullOrWhiteSpace(statusKey))
            return ValidationResult.Error(ConsoleValidationCode.StatusKeyOrIdRequired);

        if (projectKey is null && projectId is null)
        {
            return ValidationResult.Error(
                ConsoleValidationCode.ProjectKeyOrIdRequiredForStatusKey,
                nameof(command.ProjectKey));
        }

        // If ProjectKey is provided, validate that the project exists
        if (projectKey is not null)
        {
            var query = new GetProjectByKeyQuery(projectKey);
            var project = await _mediator.Send(query, token);

            if (project is null)
            {
                return ValidationResult.Error(
                    ConsoleValidationCode.ProjectKeyNotFound,
                    nameof(command.ProjectKey),
                    projectKey);
            }
        }

        return ValidationResult.Success;
    }
}
