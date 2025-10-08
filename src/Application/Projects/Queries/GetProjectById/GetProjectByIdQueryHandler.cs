using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;

namespace Lucy.Application.Projects.Queries.GetProjectById;

/// <summary>
/// Handler for getting a project by its ID.
/// </summary>
/// <param name="unitOfWork"></param>
public class GetProjectByIdQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to get a project by its ID.
    /// </summary>
    public Task<ProjectDto?> HandleAsync(GetProjectByIdQuery request, CancellationToken token = default)
        => _uow.Projects.GetByIdAsync(request.Id, token)
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
