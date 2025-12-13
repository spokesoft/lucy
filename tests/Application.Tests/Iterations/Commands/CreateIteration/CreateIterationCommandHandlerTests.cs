using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Commands.CreateIteration;
using Lucy.Application.Iterations.Repositories;
using Lucy.Application.Sequences.Repositories;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;

namespace Lucy.Application.Tests.Iterations.Commands.CreateIteration;

public class CreateIterationCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IIterationRepository> _iterationRepositoryMock;
    private readonly Mock<ISequenceRepository> _sequenceRepositoryMock;
    private readonly CreateIterationCommandHandler _handler;

    public CreateIterationCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _iterationRepositoryMock = new Mock<IIterationRepository>();
        _sequenceRepositoryMock = new Mock<ISequenceRepository>();

        _unitOfWorkMock.Setup(u => u.Iterations).Returns(_iterationRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Sequences).Returns(_sequenceRepositoryMock.Object);

        _handler = new CreateIterationCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateIteration_WhenValid()
    {
        // Arrange
        var projectId = 1L;
        var sequence = new Sequence(SequenceType.Iteration, projectId, 1, "SPRINT-");

        _sequenceRepositoryMock
            .Setup(r => r.GetByTypeAsync(projectId, SequenceType.Iteration, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sequence);

        var command = new CreateIterationCommand(projectId, "Sprint 1", "Goal", DateTime.Now, DateTime.Now.AddDays(14));

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _sequenceRepositoryMock.Verify(r => r.Update(sequence), Times.Once);
        _iterationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Iteration>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenSequenceNotFound()
    {
        // Arrange
        var projectId = 1L;

        _sequenceRepositoryMock
            .Setup(r => r.GetByTypeAsync(projectId, SequenceType.Iteration, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sequence?)null);

        var command = new CreateIterationCommand(projectId, "Sprint 1", "Goal", DateTime.Now, DateTime.Now.AddDays(14));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command, CancellationToken.None));
    }
}
