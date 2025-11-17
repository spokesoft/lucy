namespace Lucy.Application.Tickets.DTOs;

/// <summary>
/// Represents a count of tickets for a specific status.
/// </summary>
public record TicketCountByStatusDto
{
    /// <summary>
    /// Gets the status ID.
    /// </summary>
    public required long StatusId { get; init; }

    /// <summary>
    /// Gets the status key.
    /// </summary>
    public required string StatusKey { get; init; }

    /// <summary>
    /// Gets the status name.
    /// </summary>
    public required string? StatusName { get; init; }

    /// <summary>
    /// Gets the status color.
    /// </summary>
    public required string StatusColor { get; init; }

    /// <summary>
    /// Gets the count of tickets in this status.
    /// </summary>
    public required int Count { get; init; }
}
