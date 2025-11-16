using Lucy.Application.Interfaces;
using Lucy.Application.Statuses.DTOs;

namespace Lucy.Application.Statuses.Queries.GetStatusByKey;

/// <summary>
/// Handler for getting a status by its key and project ID.
/// </summary>
public class GetStatusByKeyQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<GetStatusByKeyQuery, StatusDto?>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to get a status by its key.
    /// </summary>
    public Task<StatusDto?> HandleAsync(GetStatusByKeyQuery request, CancellationToken token = default)
        => _uow.Statuses.GetByKeyAsync(request.ProjectId, request.Key.ToUpperInvariant(), token)
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
