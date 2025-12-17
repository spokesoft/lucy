namespace Lucy.Application.Comments.Queries;

/// <summary>
/// Defines the fields by which comments can be sorted or filtered.
/// </summary>
public enum CommentField
{
    /// <summary>
    /// Sort/Filter by the ID field.
    /// </summary>
    Id,

    /// <summary>
    /// Sort/Filter by the content field.
    /// </summary>
    Content,

    /// <summary>
    /// Sort/Filter by the created at field.
    /// </summary>
    CreatedAt,

    /// <summary>
    /// Sort/Filter by the updated at field.
    /// </summary>
    UpdatedAt,

    /// <summary>
    /// Sort/Filter by the project ID field.
    /// </summary>
    ProjectId,

    /// <summary>
    /// Sort/Filter by the ticket ID field.
    /// </summary>
    TicketId
}
