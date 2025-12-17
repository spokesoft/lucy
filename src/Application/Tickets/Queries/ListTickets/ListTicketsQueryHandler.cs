using Lucy.Application.Common.Interfaces;
using Lucy.Application.Tickets.DTOs;
using Lucy.Domain.Entities;

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
        var tickets = await _uow.Tickets.SearchAsync(
            request.ProjectId,
            request.StatusId,
            request.TagId,
            request.IterationId,
            request.SortBy,
            request.SortDirection,
            token);

        return [.. tickets.Select(ticket => new TicketDto
        {
            Id = ticket.Id,
            ProjectId = ticket.ProjectId,
            StatusId = ticket.StatusId,
            Key = ticket.Key,
            Number = ticket.Number,
            Title = ticket.Title,
            Description = ticket.Description,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        })];
    }
}
