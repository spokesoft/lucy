using Lucy.Application.Interfaces;

namespace Lucy.Application.Projects.Commands.DeleteProject;

/// <summary>
/// Handler for the DeleteProjectCommand.
/// </summary>
public class DeleteProjectCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteProjectCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the DeleteProjectCommand.
    /// </summary>
    public async Task HandleAsync(DeleteProjectCommand request, CancellationToken token = default)
    {
        var project = await _uow.Projects.GetByIdAsync(request.Id, token);

        if (project is null)
        {
            // This shouldn't happen if validation is done prior to handling
            throw new InvalidOperationException("Cannot delete a non-existent project.");
        }

        _uow.Projects.Remove(project);
        await _uow.SaveChangesAsync(token);
    }
}
