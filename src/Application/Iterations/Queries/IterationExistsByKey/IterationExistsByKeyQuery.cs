using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Iterations.Queries.IterationExistsByKey;

/// <summary>
/// Query to check if an iteration exists by its key.
/// </summary>
/// <param name="Key">The key of the iteration to check for.</param>
public record IterationExistsByKeyQuery(string Key) : IRequest<bool>;
