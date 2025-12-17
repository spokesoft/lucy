using Lucy.Application.Common.Interfaces;
using Lucy.Domain.Enums;

namespace Lucy.Application.Statuses.Commands.UpdateStatus;

/// <summary>
/// Update Status Command
/// </summary>
/// <param name="Id">The unique identifier of the status to be updated.</param>
/// <param name="Key">The key of the status.</param>
/// <param name="Order">The order of the status.</param>
/// <param name="Name">The name of the status.</param>
/// <param name="Description">The description of the status.</param>
/// <param name="Color">The color of the status.</param>
public record UpdateStatusCommand(
    long Id,
    string? Key,
    int? Order,
    string? Name,
    string? Description,
    Color? Color) : IRequest;
