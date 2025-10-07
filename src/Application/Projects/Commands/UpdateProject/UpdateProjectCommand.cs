using Lucy.Application.Interfaces;

namespace Lucy.Application.Projects.Commands.UpdateProject;

/// <summary>
/// Update Project Command
/// </summary>
/// <param name="Id">The unique identifier of the project to be updated.</param>
/// <param name="Key">The key of the project.</param>
/// <param name="Name">The name of the project.</param>
/// <param name="Description">The description of the project.</param>
public record UpdateProjectCommand(
    long Id,
    string? Key,
    string? Name,
    string? Description) : IRequest;
