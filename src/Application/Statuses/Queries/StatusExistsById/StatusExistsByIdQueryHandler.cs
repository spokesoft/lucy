using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Statuses.Queries.StatusExistsById;

/// <summary>
/// Handler for checking if a status exists by its ID.
/// </summary>
public class StatusExistsByIdQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<StatusExistsByIdQuery, bool>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to check if a status exists by its ID.
    /// </summary>
    public Task<bool> HandleAsync(StatusExistsByIdQuery request, CancellationToken token = default)
        => _uow.Statuses.ExistsByIdAsync(request.Id, token);
}
