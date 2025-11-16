namespace Lucy.Domain.Entities;

/// <summary>
/// A ticket entity.
/// </summary>
public class Ticket : DomainEntity<long>
{
    /// <summary>
    /// The ID of the project this ticket belongs to.
    /// </summary>
    public long ProjectId { get; private set; }

    /// <summary>
    /// The ID of the status this ticket is in.
    /// </summary>
    public long StatusId { get; private set; }

    /// <summary>
    /// The ticket key (e.g., "PROJ-123").
    /// </summary>
    public string Key { get; private set; }

    /// <summary>
    /// The title of the ticket.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// The description of the ticket.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// The project this ticket belongs to.
    /// </summary>
    public Project Project { get; private set; } = null!;

    /// <summary>
    /// The status this ticket is in.
    /// </summary>
    public Status Status { get; private set; } = null!;

    /// <summary>
    /// The comments associated with the ticket.
    /// </summary>
    public ICollection<TicketComment> Comments { get; private set; } = [];

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public Ticket(
        long projectId,
        long statusId,
        string key,
        string title,
        string? description = null)
    {
        ProjectId = projectId;
        StatusId = statusId;
        Key = null!;
        Title = null!;

        UpdateKey(key);
        UpdateTitle(title);
        UpdateDescription(description);
    }

    /// <summary>
    /// Updates the ticket key.
    /// </summary>
    public void UpdateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Ticket key cannot be null or empty.");

        if (key.Length > 20)
            throw new ArgumentException("Ticket key cannot exceed 20 characters.", nameof(key));

        Key = key;
    }

    /// <summary>
    /// Updates the ticket title.
    /// </summary>
    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Ticket title cannot be null or empty.");

        if (title.Length > 200)
            throw new ArgumentException("Ticket title cannot exceed 200 characters.", nameof(title));

        Title = title;
    }

    /// <summary>
    /// Updates the ticket description.
    /// </summary>
    public void UpdateDescription(string? description)
    {
        if (description is not null && description.Length > 5000)
            throw new ArgumentException("Ticket description cannot exceed 5000 characters.", nameof(description));

        Description = description;
    }

    /// <summary>
    /// Updates the status of the ticket.
    /// </summary>
    public void UpdateStatus(long statusId) => StatusId = statusId;
}
