namespace Lucy.Application.Projects.DTOs;

/// <summary>
/// Project DTO
/// </summary>
public class ProjectDto
{
    /// <summary>
    /// Project Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Project Key
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// Project Name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Project Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Project Creation Date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Project Update Date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
