using Lucy.Application.Interfaces;

namespace Lucy.Application.Projects.Queries.ProjectExistsById;

/// <summary>
/// Handler for checking if a project exists by its ID.
/// </summary>
public class ProjectExistsByIdQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<ProjectExistsByIdQuery, bool>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to check if a project exists by its ID.
    /// </summary>
    public Task<bool> HandleAsync(ProjectExistsByIdQuery request, CancellationToken token = default)
        => _uow.Projects.ExistsByIdAsync(request.Id, token);
}
