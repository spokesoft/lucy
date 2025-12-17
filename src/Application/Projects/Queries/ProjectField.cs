namespace Lucy.Application.Projects.Queries;

/// <summary>
/// Defines the fields by which projects can be sorted.
/// </summary>
public enum ProjectField
{
    /// <summary>
    /// Sort/Filter by the ID field.
    /// </summary>
    Id,

    /// <summary>
    /// Sort/Filter by the key field.
    /// </summary>
    Key,

    /// <summary>
    /// Sort/Filter by the name field.
    /// </summary>
    Name,

    /// <summary>
    /// Sort/Filter by the description field.
    /// </summary>
    Description,

    /// <summary>
    /// Sort/Filter by the creation date.
    /// </summary>
    CreatedAt,

    /// <summary>
    /// Sort/Filter by the last update date.
    /// </summary>
    UpdatedAt
}
