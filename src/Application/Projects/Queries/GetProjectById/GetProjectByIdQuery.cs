using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;

namespace Lucy.Application.Projects.Queries.GetProjectById;

/// <summary>
/// Query to get a project by its ID.
/// </summary>
/// <param name="Id">The ID of the project to retrieve.</param>
public record GetProjectByIdQuery(long Id) : IRequest<ProjectDto?>;
