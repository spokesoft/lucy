using Lucy.Application.Interfaces;
using Lucy.Domain.Enums;

namespace Lucy.Application.Statuses.Commands.CreateStatus;

/// <summary>
/// Command to create a new status.
/// </summary>
/// <param name="ProjectId">The ID of the project this status belongs to.</param>
/// <param name="Key">The unique key for the status.</param>
/// <param name="Order">The order of the status.</param>
/// <param name="Name">The name of the status.</param>
/// <param name="Description">A brief description of the status.</param>
/// <param name="Color">The color of the status.</param>
public record CreateStatusCommand(
    long ProjectId,
    string Key,
    int? Order,
    string? Name,
    string? Description,
    StatusColor? Color) : IRequest<long>;
