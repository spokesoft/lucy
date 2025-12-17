using Lucy.Application.Comments.DTOs;
using Lucy.Application.Comments.Queries.ListProjectComments;
using Lucy.Application.Comments.Queries.ListTicketComments;
using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Handler for the <see cref="ListCommentsCommand"/> command.
/// </summary>
internal class ListCommentsCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IViewRenderer<IEnumerable<CommentDto>> viewRenderer,
    IMediator mediator) : ICommandHandler<ListCommentsCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IViewRenderer<IEnumerable<CommentDto>> _view = viewRenderer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ListCommentsCommand command,
        CancellationToken token = default)
    {
        IEnumerable<CommentDto> comments;

        // Determine if listing comments for project or ticket
        if (command.ProjectId.HasValue)
        {
            var query = new ListProjectCommentsQuery(command.ProjectId.Value);
            comments = await _mediator.Send(query, token);
        }
        else if (command.TicketId.HasValue)
        {
            var query = new ListTicketCommentsQuery(command.TicketId.Value);
            comments = await _mediator.Send(query, token);
        }
        else if (command.Key is not null)
        {
            // Try to find as ticket key first
            var ticketQuery = new GetTicketByKeyQuery(command.Key);
            var ticket = await _mediator.Send(ticketQuery, token);

            if (ticket is not null)
            {
                var query = new ListTicketCommentsQuery(ticket.Id);
                comments = await _mediator.Send(query, token);
            }
            else
            {
                // If not a ticket, try as project key
                var projectQuery = new GetProjectIdByKeyQuery(command.Key);
                var projectId = await _mediator.Send(projectQuery, token);

                if (projectId.HasValue)
                {
                    var query = new ListProjectCommentsQuery(projectId.Value);
                    comments = await _mediator.Send(query, token);
                }
                else
                {
                    throw new InvalidOperationException($"No ticket or project found with key '{command.Key}'.");
                }
            }
        }
        else
        {
            throw new InvalidOperationException("Either Key, ProjectId, or TicketId must be provided.");
        }

        await _view.RenderAsync(comments, _console, _localizer, token);
        return ExitCode.Success;
    }
}
