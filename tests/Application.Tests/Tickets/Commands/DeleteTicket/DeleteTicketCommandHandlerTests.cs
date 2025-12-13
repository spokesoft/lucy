using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.Commands.DeleteTicket;
using Lucy.Application.Tickets.Repositories;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tickets.Commands.DeleteTicket;

public class DeleteTicketCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly DeleteTicketCommandHandler _handler;

    public DeleteTicketCommandHandlerTests()
    {
        _ticketRepositoryMock = SetupRepository(u => u.Tickets);
        _handler = new DeleteTicketCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteTicket_WhenTicketExists()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _ticketRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(ticket);

        var command = new DeleteTicketCommand(1);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _ticketRepositoryMock.Verify(repo => repo.Remove(ticket), Times.Once);
        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenTicketDoesNotExist()
    {
        // Arrange
        _ticketRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync((Ticket)null!);

        var command = new DeleteTicketCommand(1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command, CancellationToken.None));
    }
}
