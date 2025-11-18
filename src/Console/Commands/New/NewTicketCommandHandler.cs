using AppCreateTicketCommand = Lucy.Application.Tickets.Commands.CreateTicket.CreateTicketCommand;
using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Application.Statuses.Queries.ListStatuses;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Handler for the <see cref="NewTicketCommand"/> command.
/// </summary>
internal class NewTicketCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<NewTicketCommand>
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
        NewTicketCommand command,
        CancellationToken token = default)
    {
        // Resolve ProjectId
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

        // Resolve StatusId
        var statusId = command.StatusId;
        if (statusId is null)
        {
            if (command.StatusKey is not null)
            {
                var statusQuery = new GetStatusByKeyQuery(projectId.Value, command.StatusKey);
                var status = await _mediator.Send(statusQuery, token);

                if (status is null)
                {
                    throw new InvalidOperationException($"Status with key '{command.StatusKey}' not found in project.");
                }

                statusId = status.Id;
            }
            else
            {
                // No status specified, use the first status by order
                var listStatusesQuery = new ListStatusesQuery(projectId.Value);
                var statuses = await _mediator.Send(listStatusesQuery, token);

                if (statuses.Count == 0)
                {
                    throw new InvalidOperationException("Project has no statuses. Create a status first.");
                }

                statusId = statuses[0].Id;
            }
        }

        var request = new AppCreateTicketCommand(
            projectId.Value,
            statusId.Value,
            command.Title,
            command.Description);

        var id = await _mediator.Send(request, token);

        _console.MarkupLine(_localizer["Messages.CreatedTicket", command.Title, id]);

        return ExitCode.Success;
    }
}
