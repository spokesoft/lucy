using Lucy.Application.Interfaces;

namespace Lucy.Application.Iterations.Queries.IterationExistsById;

/// <summary>
/// Handler for checking if an iteration exists by its ID.
/// </summary>
public class IterationExistsByIdQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<IterationExistsByIdQuery, bool>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to check if an iteration exists by its ID.
    /// </summary>
    public Task<bool> HandleAsync(IterationExistsByIdQuery request, CancellationToken token = default)
        => _uow.Iterations.ExistsByIdAsync(request.Id, token);
}
