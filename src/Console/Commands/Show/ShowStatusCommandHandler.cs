using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Statuses.Queries.GetStatusById;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Handler for the <see cref="ShowStatusCommand"/> command.
/// </summary>
internal class ShowStatusCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IViewRenderer<StatusDto> viewRenderer,
    IMediator mediator) : ICommandHandler<ShowStatusCommand>
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
    /// The view renderer for rendering the status details.
    /// </summary>
    private readonly IViewRenderer<StatusDto> _view = viewRenderer;

    /// <summary>
    /// The mediator instance for sending commands and queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ShowStatusCommand command,
        CancellationToken token = default)
    {
        var statusId = command.Id;
        if (statusId is null)
        {
            // Need to get status by key, which requires project ID
            var projectId = command.ProjectId;
            if (projectId is null)
            {
                var projectQuery = new GetProjectIdByKeyQuery(command.ProjectKey!);
                projectId = await _mediator.Send(projectQuery, token);
            }

            var statusQuery = new GetStatusByKeyQuery(projectId!.Value, command.Key!);
            var statusByKey = await _mediator.Send(statusQuery, token);
            statusId = statusByKey?.Id;
        }

        if (statusId is null)
        {
            _console.MarkupLine($"[red]{_localizer["Error.Status.NotFound"]}[/]");
            return ExitCode.Error;
        }

        var query = new GetStatusByIdQuery(statusId.Value);
        var status = await _mediator.Send(query, token);

        if (status is null)
        {
            _console.MarkupLine($"[red]{_localizer["Error.Status.NotFound"]}[/]");
            return ExitCode.Error;
        }

        await _view.RenderAsync(status, _console, _localizer, token);
        return ExitCode.Success;
    }
}
