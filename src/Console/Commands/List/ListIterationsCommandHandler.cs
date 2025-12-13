using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.DTOs;
using Lucy.Application.Iterations.Queries.ListIterations;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Handler for the <see cref="ListIterationsCommand"/> command.
/// </summary>
internal class ListIterationsCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IViewRenderer<IEnumerable<IterationDto>> viewRenderer,
    IMediator mediator) : ICommandHandler<ListIterationsCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IViewRenderer<IEnumerable<IterationDto>> _view = viewRenderer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ListIterationsCommand command,
        CancellationToken token = default)
    {
        long projectId;

        if (command.ProjectId.HasValue)
        {
            projectId = command.ProjectId.Value;
        }
        else
        {
            // Validator ensures one is present and valid
            var id = await _mediator.Send(new GetProjectIdByKeyQuery(command.ProjectKey!), token);
            if (id is null)
            {
                // This should not happen if validation passed
                throw new InvalidOperationException(_localizer["Error.Project.NotFound"]);
            }
            projectId = id.Value;
        }

        var query = new ListIterationsQuery(projectId);
        var iterations = await _mediator.Send(query, token);

        await _view.RenderAsync(iterations, _console, _localizer, token);
        return ExitCode.Success;
    }
}
