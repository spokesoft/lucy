using Lucy.Application.Interfaces;

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

        if (request.Name != null)
            status.UpdateName(request.Name);

        if (request.Description != null)
            status.UpdateDescription(request.Description);

        if (request.Color != null)
            status.UpdateColor(request.Color.Value);

        _uow.Statuses.Update(status);
        await _uow.SaveChangesAsync(token);
    }
}
