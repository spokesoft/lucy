using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;

namespace Lucy.Application.Projects.Queries.ListProjects;

/// <summary>
/// Handler for listing all projects.
/// </summary>
public class ListProjectsQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<ListProjectsQuery, List<ProjectDto>>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to list all projects.
    /// </summary>
    public Task<List<ProjectDto>> HandleAsync(ListProjectsQuery request, CancellationToken token = default)
        => _uow.Projects.GetAllAsync(token)
            .ContinueWith(task => task.Result.Select(project => new ProjectDto
            {
                Id = project.Id,
                Key = project.Key,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            }).ToList(), token);
}
