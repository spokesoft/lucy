using Lucy.Application.Interfaces;
using Lucy.Application.Tickets.DTOs;

namespace Lucy.Application.Tickets.Queries.GetTicketById;

/// <summary>
/// Handler for getting a ticket by its ID.
/// </summary>
public class GetTicketByIdQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<GetTicketByIdQuery, TicketDto?>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to get a ticket by its ID.
    /// </summary>
    public Task<TicketDto?> HandleAsync(GetTicketByIdQuery request, CancellationToken token = default)
        => _uow.Tickets.GetByIdAsync(request.Id, token)
            .ContinueWith(task => task.Result is not null ? new TicketDto
            {
                Id = task.Result.Id,
                ProjectId = task.Result.ProjectId,
                StatusId = task.Result.StatusId,
                Key = task.Result.Key,
                Number = task.Result.Number,
                Title = task.Result.Title,
                Description = task.Result.Description,
                CreatedAt = task.Result.CreatedAt,
                UpdatedAt = task.Result.UpdatedAt
            } : null, token);
}
