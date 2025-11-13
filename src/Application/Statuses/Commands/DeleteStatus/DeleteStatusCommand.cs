using Lucy.Application.Interfaces;

namespace Lucy.Application.Statuses.Commands.DeleteStatus;

/// <summary>
/// Command to delete a status by its ID.
/// </summary>
/// <param name="Id">The unique identifier of the status to be deleted.</param>
public record DeleteStatusCommand(long Id) : IRequest;
