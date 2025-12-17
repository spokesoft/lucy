using Lucy.Application.Common.Interfaces;
using Lucy.Application.Statuses.DTOs;

namespace Lucy.Application.Statuses.Queries.GetStatusById;

/// <summary>
/// Query to get a status by its ID.
/// </summary>
/// <param name="Id">The ID of the status to retrieve.</param>
public record GetStatusByIdQuery(long Id) : IRequest<StatusDto?>;
