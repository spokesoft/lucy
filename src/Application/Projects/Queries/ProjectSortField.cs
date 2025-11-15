namespace Lucy.Application.Projects.Queries;

/// <summary>
/// Defines the fields by which projects can be sorted.
/// </summary>
public enum ProjectSortField
{
    /// <summary>
    /// Sort by the ID field.
    /// </summary>
    Id,

    /// <summary>
    /// Sort by the key field.
    /// </summary>
    Key,

    /// <summary>
    /// Sort by the name field.
    /// </summary>
    Name,

    /// <summary>
    /// Sort by the creation date.
    /// </summary>
    CreatedAt,

    /// <summary>
    /// Sort by the last update date.
    /// </summary>
    UpdatedAt
}
