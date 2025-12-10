using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.DTOs;

namespace Lucy.Application.Iterations.Queries.GetIterationByKey;

/// <summary>
/// Handler for getting an iteration by its key.
/// </summary>
public class GetIterationByKeyQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<GetIterationByKeyQuery, IterationDto?>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to get an iteration by its key.
    /// </summary>
    public Task<IterationDto?> HandleAsync(GetIterationByKeyQuery request, CancellationToken token = default)
        => _uow.Iterations.GetByKeyAsync(request.Key.ToUpperInvariant(), token)
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
