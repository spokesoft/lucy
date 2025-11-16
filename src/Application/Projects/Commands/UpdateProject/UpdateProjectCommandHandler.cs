using Lucy.Application.Interfaces;

namespace Lucy.Application.Projects.Commands.UpdateProject;

/// <summary>
/// Update Project Command Handler
/// </summary>
public class UpdateProjectCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateProjectCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the UpdateProjectCommand.
    /// </summary>
    public async Task HandleAsync(UpdateProjectCommand request, CancellationToken token = default)
    {
        var project = await _uow.Projects.GetByIdAsync(request.Id, token)
            ?? throw new InvalidOperationException("Project not found, cannot update.");

        if (request.Key is not null)
            project.UpdateKey(request.Key.ToUpperInvariant());

        if (request.Name is not null)
            project.UpdateName(request.Name);

        if (request.Description is not null)
            project.UpdateDescription(request.Description);

        _uow.Projects.Update(project);
        await _uow.SaveChangesAsync(token);
    }
}
