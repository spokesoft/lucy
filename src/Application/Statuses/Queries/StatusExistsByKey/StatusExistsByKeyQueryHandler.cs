using Lucy.Application.Interfaces;

namespace Lucy.Application.Statuses.Queries.StatusExistsByKey;

/// <summary>
/// Handler for checking if a status exists by its key and project ID.
/// </summary>
public class StatusExistsByKeyQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<StatusExistsByKeyQuery, bool>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to check if a status exists by its key and project ID.
    /// </summary>
    public Task<bool> HandleAsync(StatusExistsByKeyQuery request, CancellationToken token = default)
        => _uow.Statuses.ExistsByKeyAsync(request.ProjectId, request.Key, token);
}
