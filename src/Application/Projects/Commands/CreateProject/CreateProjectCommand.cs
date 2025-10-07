using Lucy.Application.Interfaces;

namespace Lucy.Application.Projects.Commands.CreateProject;

/// <summary>
/// Command to create a new project.
/// </summary>
/// <param name="Key">The unique key for the project.</param>
/// <param name="Name">The name of the project.</param>
/// <param name="Description">A brief description of the project.</param>
public record CreateProjectCommand(
    string Key,
    string? Name,
    string? Description) : IRequest<long>;
