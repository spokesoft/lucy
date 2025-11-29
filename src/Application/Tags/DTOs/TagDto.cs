using Lucy.Domain.Enums;

namespace Lucy.Application.Tags.DTOs;

/// <summary>
/// Tag DTO
/// </summary>
public class TagDto
{
    /// <summary>
    /// Tag Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Project Id
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Tag Key
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// Tag Label
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Tag Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Tag Color
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Tag Creation Date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Tag Update Date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
