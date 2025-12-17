using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;
using Lucy.Application.Queries;

namespace Lucy.Application.Projects.Queries.ListProjects;

/// <summary>
/// Query to list all projects.
/// </summary>
/// <param name="SortBy">The field to sort by.</param>
/// <param name="SortDirection">The direction to sort.</param>
public record ListProjectsQuery(
    ProjectField SortBy = ProjectField.Id,
    SortDirection SortDirection = SortDirection.Ascending) : IRequest<List<ProjectDto>>;
