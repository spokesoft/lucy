using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;
using Lucy.Application.Projects.Queries.GetProjectById;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tags.DTOs;
using Lucy.Application.Tags.Queries.GetTagById;
using Lucy.Application.Tags.Queries.GetTagIdByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Handler for the <see cref="ShowTagCommand"/> command.
/// </summary>
internal class ShowTagCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IViewRenderer<(TagDto, ProjectDto)> viewRenderer,
    IMediator mediator) : ICommandHandler<ShowTagCommand>
{
    /// <summary>
    /// The console instance for outputting information.
    /// </summary>
    private readonly IAnsiConsole _console = console;

    /// <summary>
    /// The localizer instance for localized strings.
    /// </summary>
    private readonly IStringLocalizer<Program> _localizer = localizer;

    /// <summary>
    /// The view renderer for rendering the tag details.
    /// </summary>
    private readonly IViewRenderer<(TagDto, ProjectDto)> _view = viewRenderer;

    /// <summary>
    /// The mediator instance for sending commands and queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ShowTagCommand command,
        CancellationToken token = default)
    {
        var tagId = command.TagId;
        if (tagId is null)
        {
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

            // Need to get tag by key, which requires project ID
            if (projectId is null)
            {
                var projectQuery = new GetProjectIdByKeyQuery(projectKey!);
                projectId = await _mediator.Send(projectQuery, token);
            }

            var tagIdQuery = new GetTagIdByKeyQuery(projectId!.Value, tagKey!);
            tagId = await _mediator.Send(tagIdQuery, token);
        }

        if (tagId is null)
        {
            _console.MarkupLine($"[red]{_localizer["Error.Tag.NotFound"]}[/]");
            return ExitCode.Error;
        }

        var query = new GetTagByIdQuery(tagId.Value);
        var tag = await _mediator.Send(query, token);

        if (tag is null)
        {
            _console.MarkupLine($"[red]{_localizer["Error.Tag.NotFound"]}[/]");
            return ExitCode.Error;
        }

        var getProjectQuery = new GetProjectByIdQuery(tag.ProjectId);
        var project = await _mediator.Send(getProjectQuery, token);

        if (project is null)
        {
            _console.MarkupLine($"[red]{_localizer["Error.Project.NotFound"]}[/]");
            return ExitCode.Error;
        }

        await _view.RenderAsync((tag, project), _console, _localizer, token);
        return ExitCode.Success;
    }
}