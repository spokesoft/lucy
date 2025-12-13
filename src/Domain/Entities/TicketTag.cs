namespace Lucy.Domain.Entities;

/// <summary>
/// Represents a tag applied to a ticket.
/// </summary>
public class TicketTag : DomainEntity<long>
{
    /// <summary>
    /// The ID of the tag.
    /// </summary>
    public long TagId { get; private set; }

    /// <summary>
    /// The tag navigational property.
    /// </summary>
    public Tag Tag { get; private set; } = null!;

    /// <summary>
    /// The ID of the ticket.
    /// </summary>
    public long TicketId { get; private set; }

    /// <summary>
    /// The ticket navigational property.
    /// </summary>
    public Ticket Ticket { get; private set; } = null!;

    /// <summary>
    /// Initializes a new instance of the TicketTag class.
    /// </summary>
    public TicketTag()
    {
    }

    /// <summary>
    /// Initializes a new instance of the TicketTag class.
    /// </summary>
    public TicketTag(Ticket ticket, Tag tag)
    {
        if (ticket is null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        if (tag is null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        Ticket = ticket;
        TicketId = ticket.Id;
        Tag = tag;
        TagId = tag.Id;
    }
}
