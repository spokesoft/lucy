using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Statuses.Commands.CreateStatus;

/// <summary>
/// Handler for the CreateStatusCommand.
/// </summary>
public class CreateStatusCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<CreateStatusCommand, long>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the CreateStatusCommand.
    /// </summary>
    public async Task<long> HandleAsync(CreateStatusCommand request, CancellationToken token = default)
    {
        int order;
        var project = await _uow.Projects.GetByIdAsync(request.ProjectId, token)
            ?? throw new InvalidOperationException("Project should exist due to prior validation.");

        if (request.Order == null)
        {
            order = project!.Statuses.Count;
        }
        else
        {
            order = request.Order.Value;
            foreach (var otherStatus in project.Statuses.Where(s => s.Order >= order))
            {
                otherStatus.UpdateOrder(otherStatus.Order + 1);
                _uow.Statuses.Update(otherStatus);
            }
        }

        var status = new Status(
            request.ProjectId,
            request.Key,
            order,
            request.Name,
            request.Description,
            request.Color);

        await _uow.Statuses.AddAsync(status, token);
        await _uow.SaveChangesAsync(token);

        return status.Id;
    }
}
