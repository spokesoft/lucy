namespace Lucy.Domain.Entities;

/// <summary>
/// Base class for comments.
/// </summary>
public abstract class Comment : DomainEntity<long>
{
    /// <summary>
    /// The content of the comment.
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    protected Comment(string content)
    {
        Content = null!;
        UpdateContent(content);
    }

    /// <summary>
    /// Updates the comment content.
    /// </summary>
    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content cannot be null or empty.");

        if (content.Length > 5000)
            throw new ArgumentException("Comment content cannot exceed 5000 characters.", nameof(content));

        Content = content;
    }
}

/// <summary>
/// A comment on a project.
/// </summary>
public class ProjectComment(
    long projectId,
    string content) : Comment(content)
{
    /// <summary>
    /// The ID of the project this comment belongs to.
    /// </summary>
    public long ProjectId { get; private set; } = projectId;

    /// <summary>
    /// The project this comment belongs to.
    /// </summary>
    public Project Project { get; private set; } = null!;
}

/// <summary>
/// A comment on a ticket.
/// </summary>
public class TicketComment(
    long ticketId,
    string content) : Comment(content)
{
    /// <summary>
    /// The ID of the ticket this comment belongs to.
    /// </summary>
    public long TicketId { get; private set; } = ticketId;

    /// <summary>
    /// The ticket this comment belongs to.
    /// </summary>
    public Ticket Ticket { get; private set; } = null!;
}
