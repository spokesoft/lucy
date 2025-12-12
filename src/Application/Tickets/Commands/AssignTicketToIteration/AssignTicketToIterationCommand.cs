using Lucy.Application.Interfaces;

namespace Lucy.Application.Tickets.Commands.AssignTicketToIteration;

/// <summary>
/// Command to assign a ticket to an iteration.
/// </summary>
/// <param name="TicketId">The ID of the ticket.</param>
/// <param name="IterationId">The ID of the iteration.</param>
public record AssignTicketToIterationCommand(long TicketId, long IterationId) : IRequest;
