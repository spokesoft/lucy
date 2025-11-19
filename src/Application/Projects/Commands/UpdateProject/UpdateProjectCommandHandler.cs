using Lucy.Application.Interfaces;
using Lucy.Domain.Enums;

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

        if (request.Key is not null && !string.Equals(project.Key, request.Key, StringComparison.OrdinalIgnoreCase))
        {
            project.UpdateKey(request.Key);

            var ticketSequence = await _uow.Sequences.GetByTypeAsync(project.Id, SequenceType.Ticket, token);
            var iterationSequence = await _uow.Sequences.GetByTypeAsync(project.Id, SequenceType.Iteration, token);

            ticketSequence?.UpdateTemplate(project.Key + "-{0}");
            iterationSequence?.UpdateTemplate(project.Key + "-S{0}");

            // Cascade rename all associated tickets if requested
            if (request.CascadeRename)
            {
                var tickets = await _uow.Tickets.GetByProjectIdAsync(project.Id, token);
                foreach (var ticket in tickets)
                {
                    ticket.UpdateKey($"{project.Key}-{ticket.Number}");
                    _uow.Tickets.Update(ticket);
                }
            }
        }

        if (request.Name is not null)
            project.UpdateName(request.Name);

        if (request.Description is not null)
            project.UpdateDescription(request.Description);

        _uow.Projects.Update(project);
        await _uow.SaveChangesAsync(token);
    }
}
