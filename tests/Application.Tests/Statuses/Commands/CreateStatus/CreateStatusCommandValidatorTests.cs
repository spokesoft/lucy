using Lucy.Application.Projects.Repositories;
using Lucy.Application.Statuses.Commands.CreateStatus;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Statuses.Commands.CreateStatus;

public class CreateStatusCommandValidatorTests : ApplicationTestBase
{
    private readonly Mock<IStatusReadOnlyRepository> _statusRepositoryMock;
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly CreateStatusCommandValidator _validator;

    public CreateStatusCommandValidatorTests()
    {
        _statusRepositoryMock = SetupReadOnlyRepository(u => u.Statuses);
        _projectRepositoryMock = SetupReadOnlyRepository(u => u.Projects);
        _validator = new CreateStatusCommandValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldValidate()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var command = new CreateStatusCommand(1, "TODO", null, "To Do", "Tasks to be done", Color.Gray);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateStatusCommand(1, "TODO", null, "To Do", "Tasks to be done", Color.Gray);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProjectId");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenKeyIsInvalid()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var command = new CreateStatusCommand(1, "", null, "To Do", "Tasks to be done", Color.Gray);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Key");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenNameIsInvalid()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var command = new CreateStatusCommand(1, "TODO", null, new string('A', 51), "Tasks to be done", Color.Gray);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenDescriptionIsInvalid()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var command = new CreateStatusCommand(1, "TODO", null, "To Do", new string('A', 101), Color.Gray);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenKeyIsNotUnique()
    {
        // Arrange
        var existingStatus = new Status(1, "TODO", 1, "Existing", "Existing Status", Color.Gray);

        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status> { existingStatus });

        var command = new CreateStatusCommand(1, "TODO", null, "To Do", "Tasks to be done", Color.Gray);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Key");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenOrderIsInvalid()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var command = new CreateStatusCommand(1, "TODO", -1, "To Do", "Tasks to be done", Color.Gray);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Order");
    }
}
