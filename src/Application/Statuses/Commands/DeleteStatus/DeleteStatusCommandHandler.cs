using Lucy.Application.Interfaces;

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

        _uow.Statuses.Remove(status);
        await _uow.SaveChangesAsync(token);
    }
}
