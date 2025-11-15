using Lucy.Application.Interfaces;
using Lucy.Application.Statuses.DTOs;

namespace Lucy.Application.Statuses.Queries.ListStatuses;

/// <summary>
/// Handler for listing all statuses for a specific project.
/// </summary>
public class ListStatusesQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<ListStatusesQuery, List<StatusDto>>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to list all statuses for a specific project.
    /// </summary>
    public Task<List<StatusDto>> HandleAsync(ListStatusesQuery request, CancellationToken token = default)
        => _uow.Statuses.GetByProjectIdAsync(request.ProjectId, request.SortBy, request.SortDirection, token)
            .ContinueWith(task => task.Result.Select(status => new StatusDto
            {
                Id = status.Id,
                ProjectId = status.ProjectId,
                Key = status.Key,
                Order = status.Order,
                Name = status.Name,
                Description = status.Description,
                Color = status.Color,
                CreatedAt = status.CreatedAt,
                UpdatedAt = status.UpdatedAt
            }).ToList(), token);
}
