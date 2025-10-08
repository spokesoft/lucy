using Lucy.Application.Interfaces;

namespace Lucy.Application.Projects.Queries.ProjectExistsById;

/// <summary>
/// Query to check if a project exists by its ID.
/// </summary>
/// <param name="Id">The ID of the project to check for.</param>
public record ProjectExistsByIdQuery(long Id) : IRequest<bool>;
