namespace Lucy.Application.Statuses.Queries;

/// <summary>
/// Defines the fields by which statuses can be sorted.
/// </summary>
public enum StatusSortField
{
    /// <summary>
    /// Sort by the ID field.
    /// </summary>
    Id,

    /// <summary>
    /// Sort by the order field.
    /// </summary>
    Order,

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
