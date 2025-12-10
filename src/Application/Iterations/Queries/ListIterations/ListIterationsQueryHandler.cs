using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.DTOs;

namespace Lucy.Application.Iterations.Queries.ListIterations;

/// <summary>
/// Handler for listing all iterations.
/// </summary>
public class ListIterationsQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<ListIterationsQuery, List<IterationDto>>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to list all iterations.
    /// </summary>
    public Task<List<IterationDto>> HandleAsync(ListIterationsQuery request, CancellationToken token = default)
        => _uow.Iterations.GetAllAsync(request.SortBy, request.SortDirection, token)
            .ContinueWith(task => task.Result.Select(iteration => new IterationDto
            {
                Id = iteration.Id,
                ProjectId = iteration.ProjectId,
                Key = iteration.Key,
                Number = iteration.Number,
                Name = iteration.Name,
                Description = iteration.Description,
                StartDate = iteration.StartDate,
                EndDate = iteration.EndDate,
                CreatedAt = iteration.CreatedAt,
                UpdatedAt = iteration.UpdatedAt
            }).ToList(), token);
}
