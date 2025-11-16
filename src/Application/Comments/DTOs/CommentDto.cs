using Lucy.Domain.Enums;

namespace Lucy.Application.Comments.DTOs;

/// <summary>
/// Comment DTO base class
/// </summary>
public abstract class CommentDto
{
    /// <summary>
    /// Comment Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Comment Content
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// Comment Type
    /// </summary>
    public CommentType CommentType { get; set; }

    /// <summary>
    /// Comment Creation Date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Comment Update Date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Project Comment DTO
/// </summary>
public class ProjectCommentDto : CommentDto
{
    /// <summary>
    /// Project Id
    /// </summary>
    public long ProjectId { get; set; }
}

/// <summary>
/// Ticket Comment DTO
/// </summary>
public class TicketCommentDto : CommentDto
{
    /// <summary>
    /// Ticket Id
    /// </summary>
    public long TicketId { get; set; }
}
