using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectByKey;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Validator for the <see cref="NewCommentCommand"/> command.
/// </summary>
internal class NewCommentCommandValidator(
    IMediator mediator) : ICommandValidator<NewCommentCommand>
{
    /// <summary>
    /// The mediator instance for sending queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        NewCommentCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Validate that content is provided
        if (string.IsNullOrWhiteSpace(command.Content))
        {
            result.AddError(
                ConsoleValidationCode.CommentContentRequired,
                nameof(command.Content));
            return result;
        }

        // Validate that either Key, ProjectId, or TicketId is provided
        if (command.Key is null && command.ProjectId is null && command.TicketId is null)
        {
            result.AddError(
                ConsoleValidationCode.CommentTargetRequired,
                nameof(command.Key));
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
                    result.AddError(
                        ConsoleValidationCode.CommentTargetNotFound,
                        nameof(command.Key),
                        command.Key);
                }
            }
        }

        return result;
    }
}
