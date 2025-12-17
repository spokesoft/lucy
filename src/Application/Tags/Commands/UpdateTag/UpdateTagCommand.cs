using Lucy.Application.Common.Interfaces;
using Lucy.Domain.Enums;

namespace Lucy.Application.Tags.Commands.UpdateTag;

/// <summary>
/// Command to update a tag.
/// </summary>
/// <param name="Id">The unique identifier of the tag to be updated.</param>
/// <param name="Key">The key of the tag.</param>
/// <param name="Label">The label of the tag.</param>
/// <param name="Description">The description of the tag.</param>
/// <param name="Color">The color of the tag.</param>
public record UpdateTagCommand(
    long Id,
    string? Key,
    string? Label,
    string? Description,
    Color? Color) : IRequest;
