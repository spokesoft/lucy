using Lucy.Application.Interfaces;
using Lucy.Domain.Enums;

namespace Lucy.Application.Tags.Commands.CreateTag;

/// <summary>
/// Command to create a new tag.
/// </summary>
/// <param name="ProjectId">The project to which the tag belongs.</param>
/// <param name="Key">The unique key for the tag.</param>
/// <param name="Label">The label of the tag.</param>
/// <param name="Description">A brief description of the tag.</param>
/// <param name="Color">The color of the tag.</param>
public record CreateTagCommand(
    long ProjectId,
    string Key,
    string? Label,
    string? Description,
    Color? Color) : IRequest<long>;
