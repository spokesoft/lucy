namespace Lucy.Application.Iterations.DTOs;

/// <summary>
/// Iteration DTO
/// </summary>
public class IterationDto
{
    /// <summary>
    /// Iteration Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Project Id
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Iteration Key
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// Iteration Number
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Iteration Name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Iteration Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Iteration Start Date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Iteration End Date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Iteration Creation Date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Iteration Update Date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
