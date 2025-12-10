using Lucy.Application.Interfaces;

namespace Lucy.Application.Iterations.Queries.GetIterationIdByKey;

/// <summary>
/// Handler for getting an iteration ID by its key.
/// </summary>
public class GetIterationIdByKeyQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<GetIterationIdByKeyQuery, long?>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to get an iteration ID by its key.
    /// </summary>
    public Task<long?> HandleAsync(GetIterationIdByKeyQuery request, CancellationToken token = default)
        => _uow.Iterations.GetByKeyAsync(request.Key.ToUpperInvariant(), token)
            .ContinueWith(task => task.Result?.Id, token);
}
