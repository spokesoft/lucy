using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.TicketTags.Commands.RemoveTicketTag;

/// <summary>
/// Command to remove a tag from a ticket.
/// </summary>
/// <param name="TagId">The unique identifier of the tag to be removed.</param>
/// <param name="TicketId">The unique identifier of the ticket from which the tag will be removed.</param>
public record RemoveTicketTagCommand(
    long TagId,
    long TicketId) : IRequest;
