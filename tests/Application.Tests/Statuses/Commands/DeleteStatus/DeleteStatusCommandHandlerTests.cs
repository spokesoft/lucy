using Lucy.Application.Statuses.Commands.DeleteStatus;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.Repositories;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Statuses.Commands.DeleteStatus;

public class DeleteStatusCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<IStatusRepository> _statusRepositoryMock;
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly DeleteStatusCommandHandler _handler;

    public DeleteStatusCommandHandlerTests()
    {
        _statusRepositoryMock = SetupRepository(u => u.Statuses);
        _ticketRepositoryMock = SetupRepository(u => u.Tickets);
        _handler = new DeleteStatusCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteStatus_WhenStatusExists()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);
        var done = new Status(2, "DONE", 2, "Done", "Completed tasks", Color.Green);

        _statusRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(status);

        _statusRepositoryMock.Setup(
            repo => repo.GetByProjectIdAsync(status.ProjectId, CancellationToken.None))
            .ReturnsAsync([status, done]);

        _statusRepositoryMock.Setup(
            repo => repo.GetByKeyAsync(status.ProjectId, status.Key, CancellationToken.None))
            .ReturnsAsync(status);

        _ticketRepositoryMock.Setup(
            repo => repo.GetByStatusIdAsync(1, CancellationToken.None)).ReturnsAsync([]);

        var command = new DeleteStatusCommand(1);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _statusRepositoryMock.Verify(repo => repo.Remove(status), Times.Once);
        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenStatusDoesNotExist()
    {
        // Arrange
        _statusRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync((Status)null!);

        var command = new DeleteStatusCommand(1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command, CancellationToken.None));
    }
}
