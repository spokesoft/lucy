using Lucy.Application.Interfaces;

namespace Lucy.Application.TicketTags.Commands.AddTicketTag;

/// <summary>
/// Command to add a tag to a ticket.
/// </summary>
/// <param name="TagId">The unique identifier of the tag to be added.</param>
/// <param name="TicketId">The unique identifier of the ticket to which the tag will be added.</param>
public record AddTicketTagCommand(
    long TagId,
    long TicketId) : IRequest;
