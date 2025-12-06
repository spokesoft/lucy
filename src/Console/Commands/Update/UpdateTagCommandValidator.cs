using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Tags.Queries.TagExistsByKey;
using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Validator for <see cref="UpdateTagCommand"/>
/// </summary>
public class UpdateTagCommandValidator(
    IMediator mediator) : ICommandValidator<UpdateTagCommand>
{
    /// <summary>
    /// The mediator instance.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        UpdateTagCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (command.TagId is null)
        {
            string? tagKey = command.TagKey;
            string? projectKey = command.ProjectKey;
            long? projectId = command.ProjectId;

            if (tagKey is null && projectKey is not null && projectId.HasValue)
            {
                // Single positional argument with --project-id: treat as tag key
                tagKey = projectKey;
                projectKey = null;
            }

            if (string.IsNullOrWhiteSpace(tagKey))
                result.AddError(new ValidationError(
                    "Tag key must be provided.",
                    nameof(command.TagKey)));

            if (projectId is null && string.IsNullOrWhiteSpace(projectKey))
                result.AddError(new ValidationError(
                    "Project key or --project-id is required.",
                    nameof(command.ProjectKey)));

            if (result.IsValid && projectId is null)
            {
                var projectExistsQuery = new ProjectExistsByKeyQuery(projectKey!);
                if (!await _mediator.Send(projectExistsQuery, token))
                {
                    result.AddError(new ValidationError(
                        $"Project with key '{projectKey}' not found.",
                        nameof(command.ProjectKey)));
                }

                if (result.IsValid)
                {
                    var projectIdQuery = new GetProjectIdByKeyQuery(projectKey!);
                    projectId = await _mediator.Send(projectIdQuery, token);
                }
            }

            if (result.IsValid && projectId is not null)
            {
                var existsQuery = new TagExistsByKeyQuery(projectId.Value, tagKey!);
                var exists = await _mediator.Send(existsQuery, token);

                if (!exists)
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
