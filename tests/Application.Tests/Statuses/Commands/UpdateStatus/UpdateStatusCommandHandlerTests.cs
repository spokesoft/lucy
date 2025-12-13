using Lucy.Application.Statuses.Commands.UpdateStatus;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Statuses.Commands.UpdateStatus;

public class UpdateStatusCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<IStatusRepository> _statusRepositoryMock;
    private readonly UpdateStatusCommandHandler _handler;

    public UpdateStatusCommandHandlerTests()
    {
        _statusRepositoryMock = SetupRepository(u => u.Statuses);
        _handler = new UpdateStatusCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateStatus_WhenValidCommandIsGiven()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Old Description", Color.Gray);

        _statusRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(status);

        var command = new UpdateStatusCommand(1, null, null, "New Name", "New Description", Color.Blue);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal("New Name", status.Name);
        Assert.Equal("New Description", status.Description);
        Assert.Equal(Color.Blue, status.Color);

        UnitOfWorkMock.Verify(
            u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenStatusDoesNotExist()
    {
        // Arrange
        _statusRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, CancellationToken.None))
            .ReturnsAsync((Status)null!);

        var command = new UpdateStatusCommand(1, null, null, "New Name", "New Description", Color.Blue);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler
            .HandleAsync(command, CancellationToken.None));
    }
}
