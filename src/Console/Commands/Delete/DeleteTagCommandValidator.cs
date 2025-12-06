using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Tags.Queries.TagExistsByKey;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Validator for <see cref="DeleteTagCommand"/>
/// </summary>
public class DeleteTagCommandValidator(
    IMediator mediator) : ICommandValidator<DeleteTagCommand>
{
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        DeleteTagCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Normalize inputs to handle the case where only tag key is provided with --project-id
        string? tagKey = command.TagKey;
        string? projectKey = command.ProjectKey;
        long? projectId = command.ProjectId;

        if (tagKey is null && projectKey is not null && projectId.HasValue)
        {
            // Single positional argument with --project-id: treat as tag key
            tagKey = projectKey;
            projectKey = null;
        }

        // Validate project identification
        if (projectId is null)
        {
            if (string.IsNullOrWhiteSpace(projectKey))
                return ValidationResult.Error(ConsoleValidationCode.ProjectKeyOrIdRequired);

            var projectExistsQuery = new ProjectExistsByKeyQuery(projectKey!);
            if (!await _mediator.Send(projectExistsQuery, token))
            {
                return ValidationResult.Error(
                    ConsoleValidationCode.ProjectKeyNotFound,
                    nameof(command.ProjectKey),
                    projectKey);
            }

            var projectIdQuery = new GetProjectIdByKeyQuery(projectKey!);
            projectId = await _mediator.Send(projectIdQuery, token);
        }

        if (command.TagId is null)
        {
            if (string.IsNullOrWhiteSpace(tagKey))
                result.AddError(new ValidationError("Tag key or --id is required.", nameof(command.TagKey)));

            if (result.IsValid)
            {
                var tagExistsQuery = new TagExistsByKeyQuery(projectId!.Value, tagKey!);
                if (!await _mediator.Send(tagExistsQuery, token))
                {
                    result.AddError(new ValidationError(
                        $"Tag with key '{tagKey}' not found in project.",
                        nameof(command.TagKey)));
                }
            }
        }

        return result;
    }
}
