using Lucy.Application.Interfaces;
using Lucy.Application.Statuses.DTOs;

namespace Lucy.Application.Statuses.Queries.GetStatusById;

/// <summary>
/// Handler for getting a status by its ID.
/// </summary>
/// <param name="unitOfWork"></param>
public class GetStatusByIdQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<GetStatusByIdQuery, StatusDto?>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to get a status by its ID.
    /// </summary>
    public Task<StatusDto?> HandleAsync(GetStatusByIdQuery request, CancellationToken token = default)
        => _uow.Statuses.GetByIdAsync(request.Id, token)
            .ContinueWith(task => task.Result is not null ? new StatusDto
            {
                Id = task.Result.Id,
                ProjectId = task.Result.ProjectId,
                Key = task.Result.Key,
                Order = task.Result.Order,
                Name = task.Result.Name,
                Description = task.Result.Description,
                Color = task.Result.Color,
                CreatedAt = task.Result.CreatedAt,
                UpdatedAt = task.Result.UpdatedAt
            } : null, token);
}
