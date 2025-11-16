using Lucy.Application.Interfaces;
using Lucy.Application.Tickets.Queries.GetTicketById;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Validator for <see cref="ShowTicketCommand"/>
/// </summary>
internal class ShowTicketCommandValidator(
    IMediator mediator) : ICommandValidator<ShowTicketCommand>
{
    /// <summary>
    /// The mediator instance for sending queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ShowTicketCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Validate that either key or ID is provided
        if (command.Key is null && command.Id is null)
        {
            result.AddError(new ValidationError(
                "Either ticket key or ID must be provided.",
                nameof(command.Key)));
            return result;
        }

        // Validate that the ticket exists
        if (command.Id.HasValue)
        {
            var ticketQuery = new GetTicketByIdQuery(command.Id.Value);
            var ticket = await _mediator.Send(ticketQuery, token);

            if (ticket is null)
            {
                result.AddError(new ValidationError(
                    $"Ticket with ID {command.Id} not found.",
                    nameof(command.Id)));
            }
        }
        else
        {
            var ticketQuery = new GetTicketByKeyQuery(command.Key!);
            var ticket = await _mediator.Send(ticketQuery, token);

            if (ticket is null)
            {
                result.AddError(new ValidationError(
                    $"Ticket with key '{command.Key}' not found.",
                    nameof(command.Key)));
            }
        }

        return result;
    }
}
