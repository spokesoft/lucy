namespace Lucy.Application.Tickets.DTOs;

/// <summary>
/// Ticket DTO
/// </summary>
public class TicketDto
{
    /// <summary>
    /// Ticket Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Project Id
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// Status Id
    /// </summary>
    public long StatusId { get; set; }

    /// <summary>
    /// Ticket Key
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// Ticket Title
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Ticket Description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ticket Creation Date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Ticket Update Date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
