using Lucy.Application.Interfaces;
using Lucy.Application.Tickets.DTOs;

namespace Lucy.Application.Tickets.Queries.ListTickets;

/// <summary>
/// Handler for listing all tickets for a specific project.
/// </summary>
public class ListTicketsQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<ListTicketsQuery, List<TicketDto>>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to list all tickets for a specific project.
    /// </summary>
    public async Task<List<TicketDto>> HandleAsync(ListTicketsQuery request, CancellationToken token = default)
    {
        var tickets = request.StatusId.HasValue
            ? await _uow.Tickets.GetByProjectIdAndStatusIdAsync(request.ProjectId, request.StatusId.Value, request.SortBy, request.SortDirection, token)
            : await _uow.Tickets.GetByProjectIdAsync(request.ProjectId, request.SortBy, request.SortDirection, token);

        return [.. tickets.Select(ticket => new TicketDto
        {
            Id = ticket.Id,
            ProjectId = ticket.ProjectId,
            StatusId = ticket.StatusId,
            Key = ticket.Key,
            Title = ticket.Title,
            Description = ticket.Description,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        })];
    }
}
