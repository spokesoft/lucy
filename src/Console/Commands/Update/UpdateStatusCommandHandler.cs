using AppUpdateStatusCommand = Lucy.Application.Statuses.Commands.UpdateStatus.UpdateStatusCommand;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;
using Spectre.Console;
using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Microsoft.Extensions.Localization;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Handler for the <see cref="UpdateStatusCommand"/> command.
/// </summary>
internal class UpdateStatusCommandHandler(
    IAnsiConsole console,
    IMediator mediator,
    IStringLocalizer<Program> localizer) : ICommandHandler<UpdateStatusCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IMediator _mediator = mediator;
    private readonly IStringLocalizer<Program> _localizer = localizer;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        UpdateStatusCommand command,
        CancellationToken token = default)
    {
        var statusId = command.StatusId;
        if (statusId is null)
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

            if (projectId is null)
            {
                var projectQuery = new GetProjectIdByKeyQuery(projectKey!);
                projectId = await _mediator.Send(projectQuery, token);
            }

            var statusQuery = new GetStatusByKeyQuery(projectId.Value, statusKey!);
            var status = await _mediator.Send(statusQuery, token);
            statusId = status?.Id;
        }

        var request = new AppUpdateStatusCommand(
            statusId!.Value,
            command.NewKey,
            command.Order,
            command.Name,
            command.Description,
            command.Color);

        await _mediator.Send(request, token);

        var message = _localizer["Messages.UpdatedStatus", statusId];
        _console.MarkupLine("[green]:check_mark:[/] " + message);

        return ExitCode.Success;
    }
}
