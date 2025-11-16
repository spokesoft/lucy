using Lucy.Application.Interfaces;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Application.Tickets.Queries.GetTicketById;
using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Validator for the <see cref="UpdateTicketCommand"/> command.
/// </summary>
internal class UpdateTicketCommandValidator(
    IMediator mediator) : ICommandValidator<UpdateTicketCommand>
{
    /// <summary>
    /// The mediator instance for sending queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        UpdateTicketCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Validate that the ticket exists
        var ticketQuery = new GetTicketByIdQuery(command.Id);
        var ticket = await _mediator.Send(ticketQuery, token);

        if (ticket is null)
        {
            result.AddError(new ValidationError(
                $"Ticket with ID {command.Id} not found.",
                nameof(command.Id)));
            return result;
        }

        // Validate that at least one property is being updated
        if (command.StatusId is null &&
            command.StatusKey is null &&
            command.Title is null &&
            command.Description is null)
        {
            result.AddError(new ValidationError(
                "At least one property must be provided to update.",
                "Command"));
            return result;
        }

        // Validate StatusKey if provided
        if (command.StatusKey is not null)
        {
            var statusQuery = new GetStatusByKeyQuery(ticket.ProjectId, command.StatusKey);
            var status = await _mediator.Send(statusQuery, token);

            if (status is null)
            {
                result.AddError(new ValidationError(
                    $"Status with key '{command.StatusKey}' not found in project.",
                    nameof(command.StatusKey)));
            }
        }

        return result;
    }
}
