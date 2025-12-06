using Lucy.Application.Interfaces;
using Lucy.Application.Tags.Queries.TagExistsById;
using Lucy.Application.Tags.Queries.TagExistsByKey;
using Lucy.Application.Tickets.Queries.GetTicketById;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Add;

/// <summary>
/// Validator for <see cref="AddTagCommand"/>.
/// </summary>
public class AddTagCommandValidator(
    IMediator mediator) : ICommandValidator<AddTagCommand>
{
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        AddTagCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (command.TicketId is null && string.IsNullOrWhiteSpace(command.TicketKey))
        {
            result.AddError(new ValidationError("Ticket key or --ticket-id is required.", nameof(command.TicketKey)));
            return result;
        }

        if (command.TagId is null && string.IsNullOrWhiteSpace(command.TagKey))
        {
            result.AddError(new ValidationError("Tag key or --tag-id is required.", nameof(command.TagKey)));
            return result;
        }

        // Resolve ticket (needed to validate tag key by project)
        var ticket = command.TicketId is null
            ? await _mediator.Send(new GetTicketByKeyQuery(command.TicketKey!), token)
            : await _mediator.Send(new GetTicketByIdQuery(command.TicketId.Value), token);

        if (ticket is null)
        {
            result.AddError(new ValidationError("Ticket not found.", nameof(command.TicketKey)));
            return result;
        }

        // Validate tag
        if (command.TagId is not null)
        {
            var exists = await _mediator.Send(new TagExistsByIdQuery(command.TagId.Value), token);
            if (!exists)
            {
                result.AddError(new ValidationError("Tag not found.", nameof(command.TagId)));
            }
            return result;
        }

        // Tag key provided, validate within the ticket's project
        var tagExists = await _mediator.Send(new TagExistsByKeyQuery(ticket.ProjectId, command.TagKey!), token);
        if (!tagExists)
        {
            result.AddError(new ValidationError($"Tag with key '{command.TagKey}' not found in project.", nameof(command.TagKey)));
        }

        return result;
    }
}
