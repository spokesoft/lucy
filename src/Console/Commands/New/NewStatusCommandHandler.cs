using AppCreateStatusCommand = Lucy.Application.Statuses.Commands.CreateStatus.CreateStatusCommand;
using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Handler for the <see cref="NewStatusCommand"/> command.
/// </summary>
internal class NewStatusCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<NewStatusCommand>
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
        NewStatusCommand command,
        CancellationToken token = default)
    {
        // Get the project ID, either from the --project-id option or by looking up the project key
        var projectId = command.ProjectId;

        if (projectId is null)
        {
            var projectQuery = new GetProjectIdByKeyQuery(command.ProjectKey);
            projectId = await _mediator.Send(projectQuery, token);
        }

        var request = new AppCreateStatusCommand(
            projectId!.Value,
            command.Key,
            command.Order,
            command.Name,
            command.Description,
            command.Color);

        var id = await _mediator.Send(request, token);

        _console.MarkupLine("[green]:check_mark:[/] " + _localizer["Messages.CreatedStatus", command.Key, id]);

        return ExitCode.Success;
    }
}
