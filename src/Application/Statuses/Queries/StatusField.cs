namespace Lucy.Application.Statuses.Queries;

/// <summary>
/// Defines the fields by which statuses can be sorted.
/// </summary>
public enum StatusField
{
    /// <summary>
    /// Sort/Filter by the ID field.
    /// </summary>
    Id,

    /// <summary>
    /// Sort/Filter by the order field.
    /// </summary>
    Order,

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
    /// Sort/Filter by the color field.
    /// </summary>
    Color,

    /// <summary>
    /// Sort/Filter by the creation date.
    /// </summary>
    CreatedAt,

    /// <summary>
    /// Sort/Filter by the last update date.
    /// </summary>
    UpdatedAt
}
