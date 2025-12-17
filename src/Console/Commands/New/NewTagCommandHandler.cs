using AppCreateTagCommand = Lucy.Application.Tags.Commands.CreateTag.CreateTagCommand;
using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Handler for the <see cref="NewTagCommand"/> command.
/// </summary>
internal class NewTagCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<NewTagCommand>
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
        NewTagCommand command,
        CancellationToken token = default)
    {
        var projectId = command.ProjectId;

        if (projectId is null)
        {
            var projectQuery = new GetProjectIdByKeyQuery(command.ProjectKey!);
            projectId = await _mediator.Send(projectQuery, token);
        }

        var request = new AppCreateTagCommand(
            projectId!.Value,
            command.Key,
            command.Label,
            command.Description,
            command.Color);

        var id = await _mediator.Send(request, token);
        var projectKey = command.ProjectKey ?? projectId!.Value.ToString();

        _console.MarkupLine("[green]:check_mark:[/] " + _localizer["Messages.CreatedTag", command.Key, projectKey, id]);

        return ExitCode.Success;
    }
}
