using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;

namespace Lucy.Application.Projects.Queries.GetProjectByKey;

/// <summary>
/// Handler for getting a project by its key.
/// </summary>
public class GetProjectByKeyQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<GetProjectByKeyQuery, ProjectDto?>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to get a project by its key.
    /// </summary>
    public Task<ProjectDto?> HandleAsync(GetProjectByKeyQuery request, CancellationToken token = default)
        => _uow.Projects.GetByKeyAsync(request.Key, token)
            .ContinueWith(task => task.Result is not null ? new ProjectDto
            {
                Id = task.Result.Id,
                Key = task.Result.Key,
                Name = task.Result.Name,
                Description = task.Result.Description,
                CreatedAt = task.Result.CreatedAt,
                UpdatedAt = task.Result.UpdatedAt
            } : null, token);
}
