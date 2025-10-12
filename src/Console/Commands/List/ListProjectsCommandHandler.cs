using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.ListProjects;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Lucy.Console.Views.Projects;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Handler for the <see cref="ListProjectsCommand"/> command.
/// </summary>
internal class ListProjectsCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<ListProjectsCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ListProjectsCommand command,
        CancellationToken token = default)
    {
        var query = new ListProjectsQuery();
        var projects = await _mediator.Send(query, token);

        _console.Write(new ListProjectsView(
            new ListProjectsViewOptions
            {
                Projects = projects,
                TotalCount = projects.Count(),
                Page = 1,
                Limit = -1
            },
            _localizer,
            _console));

        return ExitCode.Success;
    }
}
