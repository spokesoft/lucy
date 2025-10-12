using Lucy.Application.Projects.DTOs;

namespace Lucy.Console.Views.Projects;

public class ShowProjectViewOptions : ViewOptions
{
    public ProjectDto? Project { get; init; }
}
