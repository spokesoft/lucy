using Lucy.Application.Common.Interfaces;
using Lucy.Application.Iterations.DTOs;

namespace Lucy.Application.Iterations.Queries.GetIterationById;

/// <summary>
/// Handler for getting an iteration by its ID.
/// </summary>
/// <param name="unitOfWork"></param>
public class GetIterationByIdQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<GetIterationByIdQuery, IterationDto?>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to get an iteration by its ID.
    /// </summary>
    public Task<IterationDto?> HandleAsync(GetIterationByIdQuery request, CancellationToken token = default)
        => _uow.Iterations.GetByIdAsync(request.Id, token)
            .ContinueWith(task => task.Result is not null ? new IterationDto
            {
                Id = task.Result.Id,
                ProjectId = task.Result.ProjectId,
                Key = task.Result.Key,
                Number = task.Result.Number,
                Name = task.Result.Name,
                Description = task.Result.Description,
                StartDate = task.Result.StartDate,
                EndDate = task.Result.EndDate,
                CreatedAt = task.Result.CreatedAt,
                UpdatedAt = task.Result.UpdatedAt
            } : null, token);
}
