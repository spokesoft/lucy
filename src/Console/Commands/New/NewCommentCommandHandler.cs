using AppCreateProjectCommentCommand = Lucy.Application.Comments.Commands.CreateProjectComment.CreateProjectCommentCommand;
using AppCreateTicketCommentCommand = Lucy.Application.Comments.Commands.CreateTicketComment.CreateTicketCommentCommand;
using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Handler for the <see cref="NewCommentCommand"/> command.
/// </summary>
internal class NewCommentCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<NewCommentCommand>
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
        NewCommentCommand command,
        CancellationToken token = default)
    {
        // Validate content is provided
        if (string.IsNullOrWhiteSpace(command.Content))
        {
            throw new InvalidOperationException("Comment content is required. Use --content option.");
        }

        // Determine if commenting on project or ticket
        long? projectId = command.ProjectId;
        long? ticketId = command.TicketId;

        // If key is provided, resolve it
        if (command.Key is not null)
        {
            // Try to find as ticket key first
            var ticketQuery = new GetTicketByKeyQuery(command.Key);
            var ticket = await _mediator.Send(ticketQuery, token);

            if (ticket is not null)
            {
                ticketId = ticket.Id;
            }
            else
            {
                // If not a ticket, try as project key
                var projectQuery = new GetProjectIdByKeyQuery(command.Key);
                projectId = await _mediator.Send(projectQuery, token);

                if (projectId is null)
                {
                    throw new InvalidOperationException($"No ticket or project found with key '{command.Key}'.");
                }
            }
        }

        // Create the appropriate comment
        long commentId;
        if (ticketId.HasValue)
        {
            var request = new AppCreateTicketCommentCommand(ticketId.Value, command.Content);
            commentId = await _mediator.Send(request, token);
            _console.MarkupLine(_localizer["Messages.CreatedTicketComment", commentId]);
        }
        else if (projectId.HasValue)
        {
            var request = new AppCreateProjectCommentCommand(projectId.Value, command.Content);
            commentId = await _mediator.Send(request, token);
            _console.MarkupLine(_localizer["Messages.CreatedProjectComment", commentId]);
        }
        else
        {
            throw new InvalidOperationException("Either Key, ProjectId, or TicketId must be provided.");
        }

        return ExitCode.Success;
    }
}
