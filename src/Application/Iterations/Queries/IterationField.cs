namespace Lucy.Application.Iterations.Queries;

/// <summary>
/// Defines the fields by which iterations can be sorted.
/// </summary>
public enum IterationField
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
    /// Sort/Filter by the number field.
    /// </summary>
    Number,

    /// <summary>
    /// Sort/Filter by the start date.
    /// </summary>
    StartDate,

    /// <summary>
    /// Sort/Filter by the end date.
    /// </summary>
    EndDate,

    /// <summary>
    /// Sort/Filter by the creation date.
    /// </summary>
    CreatedAt,

    /// <summary>
    /// Sort/Filter by the last update date.
    /// </summary>
    UpdatedAt
}
