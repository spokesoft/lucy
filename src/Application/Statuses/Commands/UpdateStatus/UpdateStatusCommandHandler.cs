using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Statuses.Commands.UpdateStatus;

/// <summary>
/// Update Status Command Handler
/// </summary>
public class UpdateStatusCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateStatusCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the UpdateStatusCommand.
    /// </summary>
    public async Task HandleAsync(UpdateStatusCommand request, CancellationToken token = default)
    {
        var status = await _uow.Statuses.GetByIdAsync(request.Id, token)
            ?? throw new InvalidOperationException("Status should exist due to prior validation.");

        if (request.Key != null)
            status.UpdateKey(request.Key.ToUpperInvariant());

        if (request.Order.HasValue)
        {
            var oldOrder = status.Order;
            var newOrder = request.Order.Value;

            if (oldOrder != newOrder)
            {
                // Get all statuses in the same project
                var allStatuses = await _uow.Statuses.GetByProjectIdAsync(status.ProjectId, token);

                if (newOrder > oldOrder)
                {
                    // Moving down: shift statuses between old and new position up
                    foreach (var otherStatus in allStatuses
                        .Where(s => s.Id != status.Id && s.Order > oldOrder && s.Order <= newOrder))
                    {
                        otherStatus.UpdateOrder(otherStatus.Order - 1);
                        _uow.Statuses.Update(otherStatus);
                    }
                }
                else
                {
                    // Moving up: shift statuses between new and old position down
                    foreach (var otherStatus in allStatuses
                        .Where(s => s.Id != status.Id && s.Order >= newOrder && s.Order < oldOrder))
                    {
                        otherStatus.UpdateOrder(otherStatus.Order + 1);
                        _uow.Statuses.Update(otherStatus);
                    }
                }
            }

            status.UpdateOrder(newOrder);
        }

        if (request.Name != null)
            status.UpdateName(request.Name);

        if (request.Description != null)
            status.UpdateDescription(request.Description);

        if (request.Color.HasValue)
            status.UpdateColor(request.Color.Value);

        _uow.Statuses.Update(status);
        await _uow.SaveChangesAsync(token);
    }
}
