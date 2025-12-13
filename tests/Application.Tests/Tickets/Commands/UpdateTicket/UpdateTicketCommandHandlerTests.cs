using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.Commands.UpdateTicket;
using Lucy.Application.Tickets.Repositories;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tickets.Commands.UpdateTicket;

public class UpdateTicketCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly UpdateTicketCommandHandler _handler;

    public UpdateTicketCommandHandlerTests()
    {
        _ticketRepositoryMock = SetupRepository(u => u.Tickets);
        _handler = new UpdateTicketCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateStatus_WhenStatusIdIsProvided()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _ticketRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(ticket);

        var command = new UpdateTicketCommand(1, 3, null, null);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal(3L, ticket.StatusId);
        UnitOfWorkMock.Verify(
            u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateTitle_WhenTitleIsProvided()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Old Title", "This is a test ticket");

        _ticketRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(ticket);

        var command = new UpdateTicketCommand(1, null, "New Title", null);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal("New Title", ticket.Title);
        UnitOfWorkMock.Verify(
            u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateDescription_WhenDescriptionIsProvided()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "Old Description");

        _ticketRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(ticket);

        var command = new UpdateTicketCommand(1, null, null, "New Description");

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal("New Description", ticket.Description);
        UnitOfWorkMock.Verify(
            u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateAllProperties_WhenAllAreProvided()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Old Title", "Old Description");

        _ticketRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(ticket);

        var command = new UpdateTicketCommand(1, 3, "New Title", "New Description");

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal(3L, ticket.StatusId);
        Assert.Equal("New Title", ticket.Title);
        Assert.Equal("New Description", ticket.Description);
        UnitOfWorkMock.Verify(
            u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenTicketDoesNotExist()
    {
        // Arrange
        _ticketRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, CancellationToken.None))
            .ReturnsAsync((Ticket)null!);

        var command = new UpdateTicketCommand(1, 3, "New Title", "New Description");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler
            .HandleAsync(command, CancellationToken.None));
    }
}
