using Lucy.Application.Interfaces;

namespace Lucy.Application.Projects.Queries.GetProjectIdByKey;

/// <summary>
/// Query to get a project ID by its key.
/// </summary>
/// <param name="Key">The key of the project to retrieve the ID for.</param>
public record GetProjectIdByKeyQuery(string Key) : IRequest<long?>;
