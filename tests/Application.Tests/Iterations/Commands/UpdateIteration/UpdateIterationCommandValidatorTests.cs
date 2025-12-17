using Lucy.Application.Common.Interfaces;
using Lucy.Application.Iterations.Commands.UpdateIteration;
using Lucy.Application.Iterations.Repositories;
using Lucy.Application.Common.Validation;
using Lucy.Domain.Entities;
using Moq;

namespace Lucy.Application.Tests.Iterations.Commands.UpdateIteration;

public class UpdateIterationCommandValidatorTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<IIterationReadOnlyRepository> _iterationRepositoryMock;
    private readonly UpdateIterationCommandValidator _validator;

    public UpdateIterationCommandValidatorTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _iterationRepositoryMock = new Mock<IIterationReadOnlyRepository>();

        _readOnlyUnitOfWorkMock.Setup(u => u.Iterations).Returns(_iterationRepositoryMock.Object);

        _validator = new UpdateIterationCommandValidator(_readOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnSuccess_WhenValid()
    {
        // Arrange
        var iteration = new Iteration(1, "SPRINT-1", 1, "Sprint 1", "Goal", DateTime.Now, DateTime.Now.AddDays(14)) { Id = 10 };

        _iterationRepositoryMock
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(iteration);

        var command = new UpdateIterationCommand(10, "New Name", null, null, null);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnError_WhenNotFound()
    {
        // Arrange
        _iterationRepositoryMock
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Iteration?)null);

        var command = new UpdateIterationCommand(10, "New Name", null, null, null);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.IterationNotFound.ToString());
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnError_WhenNoDataToUpdate()
    {
        // Arrange
        var iteration = new Iteration(1, "SPRINT-1", 1, "Sprint 1", "Goal", DateTime.Now, DateTime.Now.AddDays(14)) { Id = 10 };

        _iterationRepositoryMock
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(iteration);

        var command = new UpdateIterationCommand(10, null, null, null, null);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.IterationNoDataToUpdate.ToString());
    }
}
