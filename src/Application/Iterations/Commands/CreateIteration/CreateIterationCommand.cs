using Lucy.Application.Interfaces;

namespace Lucy.Application.Iterations.Commands.CreateIteration;

/// <summary>
/// Command to create a new iteration.
/// </summary>
/// <param name="ProjectId">The ID of the project this iteration belongs to.</param>
/// <param name="Name">The name of the iteration.</param>
/// <param name="Description">A brief description of the iteration.</param>
/// <param name="StartDate">The start date of the iteration.</param>
/// <param name="EndDate">The end date of the iteration.</param>
public record CreateIterationCommand(
    long ProjectId,
    string? Name,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate) : IRequest<long>;
