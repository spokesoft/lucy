namespace Lucy.Application.Tickets.Queries;

/// <summary>
/// Fields that can be used to sort tickets.
/// </summary>
public enum TicketField
{
    /// <summary>
    /// Sort/Filter by ticket ID.
    /// </summary>
    Id = 0,

    /// <summary>
    /// Sort/Filter by ticket key.
    /// </summary>
    Key = 1,

    /// <summary>
    /// Sort/Filter by ticket number.
    /// </summary>
    Number = 2,

    /// <summary>
    /// Sort/Filter by ticket title.
    /// </summary>
    Title = 3,

    /// <summary>
    /// Sort/Filter by ticket description.
    /// </summary>
    Description = 8,

    /// <summary>
    /// Sort/Filter by project ID.
    /// </summary>
    ProjectId = 4,

    /// <summary>
    /// Sort/Filter by status ID.
    /// </summary>
    StatusId = 5,

    /// <summary>
    /// Sort/Filter by iteration ID.
    /// </summary>
    IterationId = 9,

    /// <summary>
    /// Sort/Filter by creation date.
    /// </summary>
    CreatedAt = 6,

    /// <summary>
    /// Sort/Filter by last update date.
    /// </summary>
    UpdatedAt = 7
}
