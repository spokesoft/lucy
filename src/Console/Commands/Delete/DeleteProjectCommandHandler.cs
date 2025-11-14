using AppDeleteProjectCommand = Lucy.Application.Projects.Commands.DeleteProject.DeleteProjectCommand;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Interfaces;
using Spectre.Console;
using Microsoft.Extensions.Localization;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Handler for the <see cref="DeleteProjectCommand"/> command.
/// </summary>
internal class DeleteProjectCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<DeleteProjectCommand>
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
        DeleteProjectCommand command,
        CancellationToken token = default)
    {
        var projectId = command.Id;
        if (projectId is null)
        {
            var query = new GetProjectIdByKeyQuery(command.Key!);
            projectId = await _mediator.Send(query, token);
        }

        var request = new AppDeleteProjectCommand(projectId!.Value);
        await _mediator.Send(request, token);

        if (command.Key is not null)
            _console.MarkupLine(_localizer["Messages.DeletedProjectWithKey", command.Key, projectId]);
        else
            _console.MarkupLine(_localizer["Messages.DeletedProjectWithId", projectId]);

        return ExitCode.Success;
    }
}
