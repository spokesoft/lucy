namespace Lucy.Application.Iterations.Queries;

/// <summary>
/// Defines the fields by which iterations can be sorted.
/// </summary>
public enum IterationSortField
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
    /// Sort by the start date.
    /// </summary>
    StartDate,

    /// <summary>
    /// Sort by the end date.
    /// </summary>
    EndDate,

    /// <summary>
    /// Sort by the creation date.
    /// </summary>
    CreatedAt,

    /// <summary>
    /// Sort by the last update date.
    /// </summary>
    UpdatedAt
}
