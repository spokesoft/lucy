using Lucy.Application.Interfaces;

namespace Lucy.Application.Iterations.Commands.UpdateIteration;

/// <summary>
/// Update Iteration Command
/// </summary>
/// <param name="Id">The unique identifier of the iteration to be updated.</param>
/// <param name="Name">The name of the iteration.</param>
/// <param name="Description">The description of the iteration.</param>
/// <param name="StartDate">The start date of the iteration.</param>
/// <param name="EndDate">The end date of the iteration.</param>
public record UpdateIterationCommand(
    long Id,
    string? Name,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate) : IRequest;
