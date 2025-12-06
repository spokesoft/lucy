using Lucy.Application.Interfaces;
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
        List<Ticket> tickets;

        if (request.TagId.HasValue && request.StatusId.HasValue)
        {
            tickets = await _uow.Tickets.GetByProjectIdStatusIdAndTagIdAsync(
                request.ProjectId,
                request.StatusId.Value,
                request.TagId.Value,
                request.SortBy,
                request.SortDirection,
                token);
        }
        else if (request.TagId.HasValue)
        {
            tickets = await _uow.Tickets.GetByProjectIdAndTagIdAsync(
                request.ProjectId,
                request.TagId.Value,
                request.SortBy,
                request.SortDirection,
                token);
        }
        else if (request.StatusId.HasValue)
        {
            tickets = await _uow.Tickets.GetByProjectIdAndStatusIdAsync(
                request.ProjectId,
                request.StatusId.Value,
                request.SortBy,
                request.SortDirection,
                token);
        }
        else
        {
            tickets = await _uow.Tickets.GetByProjectIdAsync(
                request.ProjectId,
                request.SortBy,
                request.SortDirection,
                token);
        }

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
