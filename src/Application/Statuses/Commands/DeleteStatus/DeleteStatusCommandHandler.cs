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
        throw new NotImplementedException();
    }
}
