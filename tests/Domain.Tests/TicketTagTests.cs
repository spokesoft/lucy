using Lucy.Domain.Entities;

namespace Lucy.Domain.Tests;

/// <summary>
/// Tests for the TicketTag domain entity.
/// </summary>
public class TicketTagTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        // Arrange
        var ticket = new Ticket(1L, 1L, "TICK-1", 1, "Test Ticket");
        var tag = new Tag(1L, "test-key");

        // Act
        var ticketTag = new TicketTag(ticket, tag);

        // Assert
        Assert.Equal(ticket.Id, ticketTag.TicketId);
        Assert.Equal(ticket, ticketTag.Ticket);
        Assert.Equal(tag.Id, ticketTag.TagId);
        Assert.Equal(tag, ticketTag.Tag);
    }

    [Fact]
    public void DefaultConstructor_ShouldInitializeProperties()
    {
        // Arrange & Act
        var ticketTag = new TicketTag();

        // Assert
        // Properties should be default values
        Assert.Equal(0L, ticketTag.TicketId);
        Assert.Null(ticketTag.Ticket);
        Assert.Equal(0L, ticketTag.TagId);
        Assert.Null(ticketTag.Tag);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenTicketIsNull()
    {
        // Arrange
        var tag = new Tag(1L, "test-key");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TicketTag(null!, tag));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenTagIsNull()
    {
        // Arrange
        var ticket = new Ticket(1L, 1L, "TICK-1", 1, "Test Ticket");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TicketTag(ticket, null!));
    }
}
