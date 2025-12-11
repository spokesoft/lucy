using AppCreateIterationCommand = Lucy.Application.Iterations.Commands.CreateIteration.CreateIterationCommand;
using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Handler for the <see cref="NewIterationCommand"/> command.
/// </summary>
internal class NewIterationCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<NewIterationCommand>
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
        NewIterationCommand command,
        CancellationToken token = default)
    {
        var projectId = command.ProjectId;
        if (projectId is null)
        {
            if (command.ProjectKey is null)
            {
                throw new InvalidOperationException("Either --project or --project-id must be provided.");
            }

            var projectQuery = new GetProjectIdByKeyQuery(command.ProjectKey);
            projectId = await _mediator.Send(projectQuery, token);
        }

        var request = new AppCreateIterationCommand(
            projectId.Value,
            command.Name,
            command.Description,
            command.StartDate,
            command.EndDate);

        var id = await _mediator.Send(request, token);

        _console.MarkupLine("[green]:check_mark:[/] " + _localizer["Messages.CreatedIteration", id]);

        return ExitCode.Success;
    }
}
