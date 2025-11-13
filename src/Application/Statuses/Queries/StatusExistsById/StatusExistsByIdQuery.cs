using Lucy.Application.Interfaces;

namespace Lucy.Application.Statuses.Queries.StatusExistsById;

/// <summary>
/// Query to check if a status exists by its ID.
/// </summary>
/// <param name="Id">The ID of the status to check for.</param>
public record StatusExistsByIdQuery(long Id) : IRequest<bool>;
