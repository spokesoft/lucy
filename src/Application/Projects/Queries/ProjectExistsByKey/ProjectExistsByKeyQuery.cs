using Lucy.Application.Interfaces;

namespace Lucy.Application.Projects.Queries.ProjectExistsByKey;

/// <summary>
/// Query to check if a project exists by its key.
/// </summary>
/// <param name="Key">The key of the project to check for.</param>
public record ProjectExistsByKeyQuery(string Key) : IRequest<bool>;
