using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectById;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Lucy.Console.Views.Projects;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Handler for the <see cref="ShowProjectCommand"/> command.
/// </summary>
internal class ShowProjectCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<ShowProjectCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ShowProjectCommand command,
        CancellationToken token = default)
    {
        var projectId = command.Id;
        if (projectId is null)
        {
            var getId = new GetProjectIdByKeyQuery(command.Key!);
            projectId = await _mediator.Send(getId, token);
        }
        var query = new GetProjectByIdQuery(projectId!.Value);
        var project = await _mediator.Send(query, token);

        _console.Write(new ShowProjectView(
            new ShowProjectViewOptions
            {
                Project = project,
            },
            _localizer,
            _console));

        return ExitCode.Success;
    }
}
