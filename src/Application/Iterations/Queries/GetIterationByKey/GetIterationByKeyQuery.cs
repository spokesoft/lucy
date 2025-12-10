using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.DTOs;

namespace Lucy.Application.Iterations.Queries.GetIterationByKey;

/// <summary>
/// Query to get an iteration by its key.
/// </summary>
/// <param name="Key">The key of the iteration to retrieve.</param>
public record GetIterationByKeyQuery(string Key) : IRequest<IterationDto?>;
