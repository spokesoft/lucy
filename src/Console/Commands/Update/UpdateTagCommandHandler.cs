using AppUpdateTagCommand = Lucy.Application.Tags.Commands.UpdateTag.UpdateTagCommand;
using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tags.Queries.GetTagIdByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Handler for the <see cref="UpdateTagCommand"/> command.
/// </summary>
internal class UpdateTagCommandHandler(
    IAnsiConsole console,
    IMediator mediator,
    IStringLocalizer<Program> localizer) : ICommandHandler<UpdateTagCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IMediator _mediator = mediator;
    private readonly IStringLocalizer<Program> _localizer = localizer;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        UpdateTagCommand command,
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

            if (projectId is null)
            {
                var projectQuery = new GetProjectIdByKeyQuery(projectKey!);
                projectId = await _mediator.Send(projectQuery, token);
            }

            var tagIdQuery = new GetTagIdByKeyQuery(projectId.Value, tagKey!);
            tagId = await _mediator.Send(tagIdQuery, token);
        }

        var request = new AppUpdateTagCommand(
            tagId!.Value,
            command.NewKey,
            command.Label,
            command.Description,
            command.Color);

        await _mediator.Send(request, token);

        var message = _localizer["Messages.UpdatedTag", tagId];
        _console.MarkupLine("[green]:check_mark:[/] " + message);

        return ExitCode.Success;
    }
}
