using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;

namespace Lucy.Application.Iterations.Commands.CreateIteration;

/// <summary>
/// Handler for the CreateIterationCommand.
/// </summary>
public class CreateIterationCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<CreateIterationCommand, long>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the CreateIterationCommand.
    /// </summary>
    public async Task<long> HandleAsync(CreateIterationCommand request, CancellationToken token = default)
    {
        var sequence = await _uow.Sequences.GetByTypeAsync(request.ProjectId, SequenceType.Iteration, token)
            ?? throw new InvalidOperationException("Iteration sequence not found for project.");

        var key = sequence.Next();

        var iteration = new Iteration(
            request.ProjectId,
            key,
            sequence.Value,
            request.Name,
            request.Description,
            request.StartDate,
            request.EndDate);

        _uow.Sequences.Update(sequence);
        await _uow.Iterations.AddAsync(iteration, token);
        await _uow.SaveChangesAsync(token);

        return iteration.Id;
    }
}
