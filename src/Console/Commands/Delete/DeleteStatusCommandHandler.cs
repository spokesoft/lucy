using AppDeleteStatusCommand = Lucy.Application.Statuses.Commands.DeleteStatus.DeleteStatusCommand;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Interfaces;
using Spectre.Console;
using Microsoft.Extensions.Localization;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Handler for the <see cref="DeleteStatusCommand"/> command.
/// </summary>
internal class DeleteStatusCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<DeleteStatusCommand>
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
    /// The mediator instance for sending commands and queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        DeleteStatusCommand command,
        CancellationToken token = default)
    {
        var projectId = command.ProjectId;
        if (projectId is null)
        {
            var projectQuery = new GetProjectIdByKeyQuery(command.ProjectKey!);
            projectId = await _mediator.Send(projectQuery, token);
        }

        var statusId = command.StatusId;
        if (statusId is null)
        {
            var statusQuery = new GetStatusByKeyQuery(projectId!.Value, command.StatusKey!);
            var status = await _mediator.Send(statusQuery, token);
            statusId = status!.Id;
        }

        var request = new AppDeleteStatusCommand(statusId!.Value);
        await _mediator.Send(request, token);

        if (command.ProjectKey is not null && command.StatusKey is not null)
            _console.MarkupLine("[green]:check_mark:[/] " + _localizer["Messages.DeletedStatusWithKeys", command.StatusKey, command.ProjectKey, statusId]);
        else if (command.StatusKey is not null)
            _console.MarkupLine("[green]:check_mark:[/] " + _localizer["Messages.DeletedStatusWithKey", command.StatusKey, statusId]);
        else
            _console.MarkupLine("[green]:check_mark:[/] " + _localizer["Messages.DeletedStatusWithId", statusId]);

        return ExitCode.Success;
    }
}
