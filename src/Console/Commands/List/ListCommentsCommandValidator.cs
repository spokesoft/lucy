using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectByKey;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Validator for the <see cref="ListCommentsCommand"/> command.
/// </summary>
internal class ListCommentsCommandValidator(
    IMediator mediator) : ICommandValidator<ListCommentsCommand>
{
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ListCommentsCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Validate that either Key, ProjectId, or TicketId is provided
        if (command.Key is null && command.ProjectId is null && command.TicketId is null)
        {
            result.AddError(new ValidationError(
                "Either Key, ProjectId, or TicketId must be provided.",
                nameof(command.Key)));
            return result;
        }

        // Validate Key if provided
        if (command.Key is not null)
        {
            // Try to find as ticket key first
            var ticketQuery = new GetTicketByKeyQuery(command.Key);
            var ticket = await _mediator.Send(ticketQuery, token);

            if (ticket is null)
            {
                // If not a ticket, try as project key
                var projectQuery = new GetProjectByKeyQuery(command.Key);
                var project = await _mediator.Send(projectQuery, token);

                if (project is null)
                {
                    result.AddError(new ValidationError(
                        $"No ticket or project found with key '{command.Key}'.",
                        nameof(command.Key)));
                }
            }
        }

        return result;
    }
}
