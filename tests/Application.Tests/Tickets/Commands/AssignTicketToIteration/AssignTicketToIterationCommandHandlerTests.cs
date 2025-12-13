using Lucy.Application.Iterations.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.Commands.AssignTicketToIteration;
using Lucy.Application.Tickets.Repositories;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tickets.Commands.AssignTicketToIteration;

public class AssignTicketToIterationCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly Mock<IIterationRepository> _iterationRepositoryMock;
    private readonly AssignTicketToIterationCommandHandler _handler;

    public AssignTicketToIterationCommandHandlerTests()
    {
        _ticketRepositoryMock = SetupRepository(u => u.Tickets);
        _iterationRepositoryMock = SetupRepository(u => u.Iterations);
        _handler = new AssignTicketToIterationCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldAssignTicket_WhenValid()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "Description");
        var iteration = new Iteration(1, "SPRINT-1", 1, "Sprint 1", "Goal", DateTime.Now, DateTime.Now.AddDays(14));
        iteration.Id = 10;

        _ticketRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(ticket);
        _iterationRepositoryMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(iteration);

        var command = new AssignTicketToIterationCommand(1, 10);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal(10, ticket.IterationId);
        _ticketRepositoryMock.Verify(r => r.Update(ticket), Times.Once);
        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenTicketNotFound()
    {
        // Arrange
        _ticketRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Ticket)null!);

        var command = new AssignTicketToIterationCommand(1, 10);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenIterationNotFound()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "Description");
        _ticketRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(ticket);
        _iterationRepositoryMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync((Iteration)null!);

        var command = new AssignTicketToIterationCommand(1, 10);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenProjectIdsMismatch()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "Description");
        var iteration = new Iteration(2, "SPRINT-1", 1, "Sprint 1", "Goal", DateTime.Now, DateTime.Now.AddDays(14)); // Different ProjectId
        iteration.Id = 10;

        _ticketRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(ticket);
        _iterationRepositoryMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(iteration);

        var command = new AssignTicketToIterationCommand(1, 10);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command, CancellationToken.None));
    }
}
