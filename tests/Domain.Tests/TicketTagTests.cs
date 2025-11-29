using Lucy.Domain.Entities;

namespace Lucy.Domain.Tests;

public class TicketTagTests
{
    [Fact]
    public void TicketTag_Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        var ticket = new Ticket(1L, 1L, "TICK-1", 1, "Test Ticket");
        var tag = new Tag(1L, "test-key");

        var ticketTag = new TicketTag(ticket, tag);

        Assert.Equal(ticket.Id, ticketTag.TicketId);
        Assert.Equal(ticket, ticketTag.Ticket);
        Assert.Equal(tag.Id, ticketTag.TagId);
        Assert.Equal(tag, ticketTag.Tag);
    }

    [Fact]
    public void TicketTag_DefaultConstructor_ShouldInitializeProperties()
    {
        var ticketTag = new TicketTag();

        // Properties should be default values
        Assert.Equal(0L, ticketTag.TicketId);
        Assert.Null(ticketTag.Ticket);
        Assert.Equal(0L, ticketTag.TagId);
        Assert.Null(ticketTag.Tag);
    }
}