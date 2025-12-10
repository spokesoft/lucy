using Lucy.Application.Interfaces;

namespace Lucy.Application.Iterations.Commands.UpdateIteration;

/// <summary>
/// Update Iteration Command Handler
/// </summary>
public class UpdateIterationCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateIterationCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the UpdateIterationCommand.
    /// </summary>
    public async Task HandleAsync(UpdateIterationCommand request, CancellationToken token = default)
    {
        var iteration = await _uow.Iterations.GetByIdAsync(request.Id, token)
            ?? throw new InvalidOperationException("Iteration not found, cannot update.");

        if (request.Name is not null)
            iteration.UpdateName(request.Name);

        if (request.Description is not null)
            iteration.UpdateDescription(request.Description);

        if (request.StartDate is not null)
            iteration.UpdateStartDate(request.StartDate);

        if (request.EndDate is not null)
            iteration.UpdateEndDate(request.EndDate);

        _uow.Iterations.Update(iteration);
        await _uow.SaveChangesAsync(token);
    }
}
