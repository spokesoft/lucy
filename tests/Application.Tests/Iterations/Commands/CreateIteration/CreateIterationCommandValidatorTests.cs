using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Commands.CreateIteration;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Validation;
using Moq;

namespace Lucy.Application.Tests.Iterations.Commands.CreateIteration;

public class CreateIterationCommandValidatorTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly CreateIterationCommandValidator _validator;

    public CreateIterationCommandValidatorTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _projectRepositoryMock = new Mock<IProjectReadOnlyRepository>();

        _readOnlyUnitOfWorkMock.Setup(u => u.Projects).Returns(_projectRepositoryMock.Object);

        _validator = new CreateIterationCommandValidator(_readOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnSuccess_WhenValid()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(r => r.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateIterationCommand(1, "Sprint 1", "Goal", DateTime.Now, DateTime.Now.AddDays(14));

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnError_WhenProjectNotFound()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(r => r.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateIterationCommand(1, "Sprint 1", "Goal", DateTime.Now, DateTime.Now.AddDays(14));

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.ProjectNotFound.ToString());
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnError_WhenNameIsInvalid()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(r => r.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateIterationCommand(1, new string('a', 101), "Goal", DateTime.Now, DateTime.Now.AddDays(14));

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.IterationNameLength.ToString());
    }
}
