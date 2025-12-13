using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Iterations.Queries.GetProjectIdFromIteration;

/// <summary>
/// Handler for getting the project ID from an iteration ID or key.
/// </summary>
public class GetProjectIdFromIterationQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<GetProjectIdFromIterationQuery, long?>
{
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to get the project ID from an iteration ID or key.
    /// </summary>
    public async Task<long?> HandleAsync(GetProjectIdFromIterationQuery request, CancellationToken token = default)
    {
        Iteration? iteration = null;

        if (request.IterationId.HasValue)
        {
            iteration = await _uow.Iterations.GetByIdAsync(request.IterationId.Value, token);
        }
        else if (!string.IsNullOrWhiteSpace(request.IterationKey))
        {
            iteration = await _uow.Iterations.GetByKeyAsync(request.IterationKey.ToUpperInvariant(), token);
        }

        return iteration?.ProjectId;
    }
}
