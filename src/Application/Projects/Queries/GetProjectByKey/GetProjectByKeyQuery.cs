using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;

namespace Lucy.Application.Projects.Queries.GetProjectByKey;

/// <summary>
/// Query to get a project by its key.
/// </summary>
/// <param name="Key">The key of the project to retrieve.</param>
public record GetProjectByKeyQuery(string Key) : IRequest<ProjectDto?>;
