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
        // Handle ambiguous case: if ProjectKey has a value and ProjectId is set,
        // then we're using single positional argument with --project-id option
        // In this case, ProjectKey is actually part of the status key argument handling
        var projectId = command.ProjectId;
        var projectKey = command.ProjectKey;

        if (projectId is null)
        {
            if (projectKey is null)
            {
                // This shouldn't happen due to validation, but handle it
                throw new InvalidOperationException("Either ProjectKey or ProjectId must be provided.");
            }

            var projectQuery = new GetProjectIdByKeyQuery(projectKey);
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

        if (projectKey is not null)
            _console.MarkupLine(_localizer["Messages.CreatedStatusWithKeys", command.Key, projectKey, id]);
        else
            _console.MarkupLine(_localizer["Messages.CreatedStatusWithKey", command.Key, id]);

        return ExitCode.Success;
    }
}
