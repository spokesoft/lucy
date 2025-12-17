using Lucy.Application.Common.Interfaces;
using Lucy.Application.Iterations.Commands.DeleteIteration;
using Lucy.Application.Iterations.Repositories;
using Lucy.Application.Common.Validation;
using Moq;

namespace Lucy.Application.Tests.Iterations.Commands.DeleteIteration;

public class DeleteIterationCommandValidatorTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<IIterationReadOnlyRepository> _iterationRepositoryMock;
    private readonly DeleteIterationCommandValidator _validator;

    public DeleteIterationCommandValidatorTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _iterationRepositoryMock = new Mock<IIterationReadOnlyRepository>();

        _readOnlyUnitOfWorkMock.Setup(u => u.Iterations).Returns(_iterationRepositoryMock.Object);

        _validator = new DeleteIterationCommandValidator(_readOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnSuccess_WhenFound()
    {
        // Arrange
        _iterationRepositoryMock
            .Setup(r => r.ExistsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new DeleteIterationCommand(10);

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
            .Setup(r => r.ExistsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new DeleteIterationCommand(10);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.IterationNotFound.ToString());
    }
}
