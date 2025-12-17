using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Iterations.Commands.DeleteIteration;

/// <summary>
/// Handler for the DeleteIterationCommand.
/// </summary>
public class DeleteIterationCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteIterationCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the DeleteIterationCommand.
    /// </summary>
    public async Task HandleAsync(DeleteIterationCommand request, CancellationToken token = default)
    {
        var iteration = await _uow.Iterations.GetByIdAsync(request.Id, token)
            ?? throw new InvalidOperationException("Iteration not found, cannot delete.");

        _uow.Iterations.Remove(iteration);
        await _uow.SaveChangesAsync(token);
    }
}
