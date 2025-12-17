using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Statuses.Commands.DeleteStatus;

/// <summary>
/// Command to delete a status by its ID.
/// </summary>
/// <param name="Id">The unique identifier of the status to be deleted.</param>
/// <param name="ReassignTo">The key of the status to reassign tickets to.</param>
/// <param name="ReassignToId">The ID of the status to reassign tickets to.</param>
public record DeleteStatusCommand(
    long Id,
    string? ReassignTo = null,
    long? ReassignToId = null) : IRequest;
