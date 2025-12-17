using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Iterations.Queries.IterationExistsByKey;

/// <summary>
/// Handler for checking if an iteration exists by its key.
/// </summary>
public class IterationExistsByKeyQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<IterationExistsByKeyQuery, bool>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to check if an iteration exists by its key.
    /// </summary>
    public Task<bool> HandleAsync(IterationExistsByKeyQuery request, CancellationToken token = default)
        => _uow.Iterations.ExistsByKeyAsync(request.Key.ToUpperInvariant(), token);
}
