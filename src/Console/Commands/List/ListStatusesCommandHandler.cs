using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Statuses.Queries.ListStatuses;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Handler for the <see cref="ListStatusesCommand"/> command.
/// </summary>
internal class ListStatusesCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IViewRenderer<IEnumerable<StatusDto>> viewRenderer,
    IMediator mediator) : ICommandHandler<ListStatusesCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IViewRenderer<IEnumerable<StatusDto>> _view = viewRenderer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ListStatusesCommand command,
        CancellationToken token = default)
    {
        // Resolve project ID from key if needed
        var projectId = command.Id ?? await _mediator.Send(
            new GetProjectIdByKeyQuery(command.Key!), token);

        var query = new ListStatusesQuery(projectId.Value);
        var statuses = await _mediator.Send(query, token);

        await _view.RenderAsync(statuses, _console, _localizer, token);
        return ExitCode.Success;
    }
}
