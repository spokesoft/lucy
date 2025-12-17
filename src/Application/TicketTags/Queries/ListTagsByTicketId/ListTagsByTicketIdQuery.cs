using Lucy.Application.Common.Interfaces;
using Lucy.Application.Tags.DTOs;

namespace Lucy.Application.TicketTags.Queries.ListTagsByTicketId;

/// <summary>
/// Query to list tags attached to a ticket.
/// </summary>
/// <param name="TicketId">The ticket identifier.</param>
public record ListTagsByTicketIdQuery(long TicketId) : IRequest<IEnumerable<TagDto>>;
