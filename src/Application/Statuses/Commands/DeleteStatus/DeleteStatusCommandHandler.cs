using Lucy.Application.Interfaces;
using Lucy.Application.Queries;
using Lucy.Application.Statuses.Queries;

namespace Lucy.Application.Statuses.Commands.DeleteStatus;

/// <summary>
/// Handler for the DeleteStatusCommand.
/// </summary>
public class DeleteStatusCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteStatusCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the DeleteStatusCommand.
    /// </summary>
    public async Task HandleAsync(DeleteStatusCommand request, CancellationToken token = default)
    {
        var status = await _uow.Statuses.GetByIdAsync(request.Id, token)
            ?? throw new InvalidOperationException("Status should exist due to prior validation.");

        var tickets = await _uow.Tickets.GetByStatusIdAsync(request.Id, token);

        if (tickets.Count > 0)
        {
            long reassignToId;
            if (request.ReassignToId.HasValue)
            {
                reassignToId = request.ReassignToId.Value;
            }
            else if (!string.IsNullOrWhiteSpace(request.ReassignTo))
            {
                var reassignStatus = await _uow.Statuses.GetByKeyAsync(status.ProjectId, request.ReassignTo, token)
                    ?? throw new InvalidOperationException("ReassignTo status not found.");

                reassignToId = reassignStatus.Id;
            }
            else
            {
                var statuses = await _uow.Statuses.GetByProjectIdAsync(
                    status.ProjectId,
                    StatusSortField.Order,
                    SortDirection.Ascending,
                    token);

                var first = statuses.FirstOrDefault(s => s.Id != status.Id)
                    ?? throw new InvalidOperationException("No statuses available to reassign tickets.");

                reassignToId = first.Id;
            }

            foreach (var ticket in tickets)
            {
                ticket.UpdateStatus(reassignToId);
                _uow.Tickets.Update(ticket);
            }
        }

        _uow.Statuses.Remove(status);
        await _uow.SaveChangesAsync(token);
    }
}
