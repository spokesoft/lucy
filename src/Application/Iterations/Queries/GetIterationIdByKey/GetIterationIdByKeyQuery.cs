using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Iterations.Queries.GetIterationIdByKey;

/// <summary>
/// Query to get an iteration ID by its key.
/// </summary>
/// <param name="Key">The key of the iteration to retrieve the ID for.</param>
public record GetIterationIdByKeyQuery(string Key) : IRequest<long?>;
