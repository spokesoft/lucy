using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.Commands.UnassignTicketFromIteration;
using Lucy.Application.Tickets.Repositories;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tickets.Commands.UnassignTicketFromIteration;

public class UnassignTicketFromIterationCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly UnassignTicketFromIterationCommandHandler _handler;

    public UnassignTicketFromIterationCommandHandlerTests()
    {
        _ticketRepositoryMock = SetupRepository(u => u.Tickets);
        _handler = new UnassignTicketFromIterationCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUnassignTicket_WhenValid()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "Description");
        ticket.SetIteration(10);

        _ticketRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(ticket);

        var command = new UnassignTicketFromIterationCommand(1, 10);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Null(ticket.IterationId);
        _ticketRepositoryMock.Verify(r => r.Update(ticket), Times.Once);
        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenTicketNotFound()
    {
        // Arrange
        _ticketRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Ticket)null!);

        var command = new UnassignTicketFromIterationCommand(1, 10);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenTicketNotAssignedToIteration()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "Description");
        ticket.SetIteration(20); // Different Iteration

        _ticketRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(ticket);

        var command = new UnassignTicketFromIterationCommand(1, 10);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command, CancellationToken.None));
    }
}
