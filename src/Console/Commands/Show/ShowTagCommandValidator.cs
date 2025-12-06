using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectByKey;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Validator for <see cref="ShowTagCommand"/>
/// </summary>
internal class ShowTagCommandValidator(
    IMediator mediator) : ICommandValidator<ShowTagCommand>
{
    /// <summary>
    /// The mediator instance for sending queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ShowTagCommand command,
        CancellationToken token = default)
    {
        // If using tag ID, no other validation needed
        if (command.TagId is not null)
            return ValidationResult.Success;

        // Handle ambiguous case: if TagKey is null but ProjectKey has a value and ProjectId is set,
        // then ProjectKey is actually the tag key (single positional argument with --project-id option)
        string? tagKey = command.TagKey;
        string? projectKey = command.ProjectKey;
        long? projectId = command.ProjectId;

        if (tagKey is null && projectKey is not null && projectId.HasValue)
        {
            // Single positional argument with --project-id: treat as tag key
            tagKey = projectKey;
            projectKey = null;
        }

        // If using tag key, need project key or ID
        if (string.IsNullOrWhiteSpace(tagKey))
            return ValidationResult.Error("Tag key or --id is required.");

        if (projectKey is null && projectId is null)
        {
            return ValidationResult.Error(
                ConsoleValidationCode.ProjectKeyOrIdRequired,
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
