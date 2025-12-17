using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Iterations.Queries.IterationExistsById;

/// <summary>
/// Query to check if an iteration exists by its ID.
/// </summary>
/// <param name="Id">The ID of the iteration to check for.</param>
public record IterationExistsByIdQuery(long Id) : IRequest<bool>;
