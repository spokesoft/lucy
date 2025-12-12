using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.DTOs;
using Lucy.Application.Iterations.Queries.GetIterationById;
using Lucy.Application.Iterations.Queries.GetIterationIdByKey;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.GetTicketCountsByIterationId;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Handler for the <see cref="ShowIterationCommand"/> command.
/// </summary>
internal class ShowIterationCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IViewRenderer<(IterationDto, IEnumerable<TicketCountByStatusDto>)> viewRenderer,
    IMediator mediator) : ICommandHandler<ShowIterationCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IViewRenderer<(IterationDto, IEnumerable<TicketCountByStatusDto>)> _view = viewRenderer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ShowIterationCommand command,
        CancellationToken token = default)
    {
        var iterationId = command.Id;
        if (iterationId is null)
        {
            var getId = new GetIterationIdByKeyQuery(command.Key!);
            iterationId = await _mediator.Send(getId, token);
        }
        var query = new GetIterationByIdQuery(iterationId!.Value);
        var iteration = await _mediator.Send(query, token);

        if (iteration is null)
        {
            _console.MarkupLine($"[red]{_localizer["Error.Iteration.NotFound"]}[/]");
            return ExitCode.Error;
        }

        var ticketCountsQuery = new GetTicketCountsByIterationIdQuery(iteration.Id);
        var ticketCounts = await _mediator.Send(ticketCountsQuery, token);

        await _view.RenderAsync((iteration, ticketCounts), _console, _localizer, token);

        return ExitCode.Success;
    }
}
