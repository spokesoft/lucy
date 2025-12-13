using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Commands.DeleteIteration;
using Lucy.Application.Iterations.Repositories;
using Lucy.Domain.Entities;
using Moq;

namespace Lucy.Application.Tests.Iterations.Commands.DeleteIteration;

public class DeleteIterationCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IIterationRepository> _iterationRepositoryMock;
    private readonly DeleteIterationCommandHandler _handler;

    public DeleteIterationCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _iterationRepositoryMock = new Mock<IIterationRepository>();

        _unitOfWorkMock.Setup(u => u.Iterations).Returns(_iterationRepositoryMock.Object);

        _handler = new DeleteIterationCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteIteration_WhenFound()
    {
        // Arrange
        var iteration = new Iteration(1, "SPRINT-1", 1, "Sprint 1", "Goal", DateTime.Now, DateTime.Now.AddDays(14)) { Id = 10 };

        _iterationRepositoryMock
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(iteration);

        var command = new DeleteIterationCommand(10);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _iterationRepositoryMock.Verify(r => r.Remove(iteration), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenNotFound()
    {
        // Arrange
        _iterationRepositoryMock
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Iteration?)null);

        var command = new DeleteIterationCommand(10);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command, CancellationToken.None));
    }
}
