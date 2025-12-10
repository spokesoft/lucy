using Lucy.Application.Interfaces;

namespace Lucy.Application.Iterations.Commands.DeleteIteration;

/// <summary>
/// Command to delete an iteration by its ID.
/// </summary>
/// <param name="Id">The unique identifier of the iteration to be deleted.</param>
public record DeleteIterationCommand(long Id) : IRequest;
