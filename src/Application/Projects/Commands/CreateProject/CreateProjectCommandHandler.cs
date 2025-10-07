using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Projects.Commands.CreateProject;

/// <summary>
/// Handler for the CreateProjectCommand.
/// </summary>
public class CreateProjectCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProjectCommand, long>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the CreateProjectCommand.
    /// </summary>
    public async Task<long> HandleAsync(CreateProjectCommand request, CancellationToken token = default)
    {
        var project = new Project
        {
            Key = request.Key.ToUpperInvariant(),
            Name = request.Name,
            Description = request.Description
        };

        await _uow.Projects.AddAsync(project, token);
        await _uow.SaveChangesAsync(token);
        return project.Id;
    }
}
