using Lucy.Application.Interfaces;

namespace Lucy.Application.Projects.Queries.ProjectExistsByKey;

/// <summary>
/// Handler for checking if a project exists by its key.
/// </summary>
public class ProjectExistsByKeyQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<ProjectExistsByKeyQuery, bool>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to check if a project exists by its key.
    /// </summary>
    public Task<bool> HandleAsync(ProjectExistsByKeyQuery request, CancellationToken token = default)
        => _uow.Projects.ExistsByKeyAsync(request.Key, token);
}
