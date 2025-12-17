namespace Lucy.Application.Tags.Queries;

/// <summary>
/// Defines the fields by which tags can be sorted or filtered.
/// </summary>
public enum TagField
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
    /// Sort/Filter by the label field.
    /// </summary>
    Label,

    /// <summary>
    /// Sort/Filter by the description field.
    /// </summary>
    Description,

    /// <summary>
    /// Sort/Filter by the color field.
    /// </summary>
    Color,

    /// <summary>
    /// Sort/Filter by the project ID field.
    /// </summary>
    ProjectId,

    /// <summary>
    /// Sort/Filter by the created at field.
    /// </summary>
    CreatedAt,

    /// <summary>
    /// Sort/Filter by the updated at field.
    /// </summary>
    UpdatedAt
}
