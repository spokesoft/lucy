using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Tickets.Commands.UnassignTicketFromIteration;

/// <summary>
/// Command to unassign a ticket from an iteration.
/// </summary>
/// <param name="TicketId">The ID of the ticket.</param>
/// <param name="IterationId">The ID of the iteration.</param>
public record UnassignTicketFromIterationCommand(long TicketId, long IterationId) : IRequest;
