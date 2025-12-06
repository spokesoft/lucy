using AppDeleteTagCommand = Lucy.Application.Tags.Commands.DeleteTag.DeleteTagCommand;
using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tags.Queries.GetTagIdByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Handler for the <see cref="DeleteTagCommand"/> command.
/// </summary>
internal class DeleteTagCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<DeleteTagCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        DeleteTagCommand command,
        CancellationToken token = default)
    {
        string? tagKey = command.TagKey;
        string? projectKey = command.ProjectKey;
        var projectId = command.ProjectId;

        if (tagKey is null && projectKey is not null && projectId.HasValue)
        {
            // Single positional argument with --project-id: treat as tag key
            tagKey = projectKey;
            projectKey = null;
        }

        if (projectId is null)
        {
            var projectQuery = new GetProjectIdByKeyQuery(projectKey!);
            projectId = await _mediator.Send(projectQuery, token);
        }

        var tagId = command.TagId;
        if (tagId is null)
        {
            var tagIdQuery = new GetTagIdByKeyQuery(projectId!.Value, tagKey!);
            tagId = await _mediator.Send(tagIdQuery, token);
        }

        var request = new AppDeleteTagCommand(tagId!.Value);
        await _mediator.Send(request, token);

        _console.MarkupLine("[green]:check_mark:[/] " + _localizer["Messages.DeletedTag", tagId]);

        return ExitCode.Success;
    }
}
