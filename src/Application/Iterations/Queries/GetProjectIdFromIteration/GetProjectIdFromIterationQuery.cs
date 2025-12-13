using Lucy.Application.Interfaces;

namespace Lucy.Application.Iterations.Queries.GetProjectIdFromIteration;

/// <summary>
/// Query to get the project ID from an iteration ID or key.
/// </summary>
/// <param name="IterationId">The ID of the iteration.</param>
/// <param name="IterationKey">The key of the iteration.</param>
public record GetProjectIdFromIterationQuery(long? IterationId, string? IterationKey) : IRequest<long?>;
