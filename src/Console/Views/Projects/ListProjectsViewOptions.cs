using Lucy.Application.Projects.DTOs;

namespace Lucy.Console.Views.Projects;

public class ListProjectsViewOptions : ViewOptions
{
    public int TotalCount { get; init; }
    public int Page { get; init; } = 1;
    public int Limit { get; init; } = -1;
    public IEnumerable<ProjectDto> Projects { get; init; } = [];
}
