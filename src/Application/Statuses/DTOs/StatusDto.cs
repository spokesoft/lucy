using Lucy.Domain.Enums;

namespace Lucy.Application.Statuses.DTOs;

/// <summary>
/// Status DTO
/// </summary>
public class StatusDto
{
    /// <summary>
    /// Status Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Project Id
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Status Key
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// Status Order
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Status Name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Status Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Status Color
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Status Creation Date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Status Update Date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
