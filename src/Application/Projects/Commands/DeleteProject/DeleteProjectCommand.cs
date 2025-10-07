using Lucy.Application.Interfaces;

namespace Lucy.Application.Projects.Commands.DeleteProject;

/// <summary>
/// Command to delete a project by its ID.
/// </summary>
/// <param name="Id">The unique identifier of the project to be deleted.</param>
public record DeleteProjectCommand(long Id) : IRequest;
