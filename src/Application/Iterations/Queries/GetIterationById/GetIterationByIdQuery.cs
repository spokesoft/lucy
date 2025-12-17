using Lucy.Application.Common.Interfaces;
using Lucy.Application.Iterations.DTOs;

namespace Lucy.Application.Iterations.Queries.GetIterationById;

/// <summary>
/// Query to get an iteration by its ID.
/// </summary>
/// <param name="Id">The ID of the iteration to retrieve.</param>
public record GetIterationByIdQuery(long Id) : IRequest<IterationDto?>;
