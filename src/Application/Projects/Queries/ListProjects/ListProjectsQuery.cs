using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;

namespace Lucy.Application.Projects.Queries.ListProjects;

/// <summary>
/// Query to list all projects.
/// </summary>
public record ListProjectsQuery : IRequest<List<ProjectDto>>;
