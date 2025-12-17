using Lucy.Application.Common.Interfaces;
using Lucy.Application.Tickets.Queries.GetTicketById;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Application.Common.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Validator for the <see cref="DeleteTicketCommand"/> command.
/// </summary>
internal class DeleteTicketCommandValidator(
    IMediator mediator) : ICommandValidator<DeleteTicketCommand>
{
    /// <summary>
    /// The mediator instance for sending queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        DeleteTicketCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Validate that either key or ID is provided
        if (command.Key is null && command.Id is null)
        {
            result.AddError(ValidationResult.Error(
                ConsoleValidationCode.TicketKeyOrIdRequired,
                nameof(command.Key)).Errors.First());
            return result;
        }

        // Validate that the ticket exists
        if (command.Id.HasValue)
        {
            var ticketQuery = new GetTicketByIdQuery(command.Id.Value);
            var ticket = await _mediator.Send(ticketQuery, token);

            if (ticket is null)
            {
                result.AddError(ValidationResult.Error(
                    ConsoleValidationCode.TicketNotFound,
                    nameof(command.Id)).Errors.First());
            }
        }
        else
        {
            var ticketQuery = new GetTicketByKeyQuery(command.Key!);
            var ticket = await _mediator.Send(ticketQuery, token);

            if (ticket is null)
            {
                result.AddError(ValidationResult.Error(
                    ConsoleValidationCode.TicketNotFound,
                    nameof(command.Key)).Errors.First());
            }
        }

        return result;
    }
}
