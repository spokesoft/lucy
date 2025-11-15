namespace Lucy.Application.Tickets.Queries;

/// <summary>
/// Fields that can be used to sort tickets.
/// </summary>
public enum TicketSortField
{
    /// <summary>
    /// Sort by ticket ID.
    /// </summary>
    Id = 0,

    /// <summary>
    /// Sort by ticket key.
    /// </summary>
    Key = 1,

    /// <summary>
    /// Sort by ticket title.
    /// </summary>
    Title = 2,

    /// <summary>
    /// Sort by project ID.
    /// </summary>
    ProjectId = 3,

    /// <summary>
    /// Sort by status ID.
    /// </summary>
    StatusId = 4,

    /// <summary>
    /// Sort by creation date.
    /// </summary>
    CreatedAt = 5,

    /// <summary>
    /// Sort by last update date.
    /// </summary>
    UpdatedAt = 6
}
