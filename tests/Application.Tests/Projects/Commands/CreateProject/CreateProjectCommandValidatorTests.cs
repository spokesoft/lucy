using Lucy.Application.Projects.Commands.CreateProject;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Projects.Commands.CreateProject;

public class CreateProjectCommandValidatorTests : ApplicationTestBase
{
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly CreateProjectCommandValidator _validator;

    public CreateProjectCommandValidatorTests()
    {
        _projectRepositoryMock = SetupReadOnlyRepository(u => u.Projects);
        _validator = new CreateProjectCommandValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldValidate()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateProjectCommand("VALID-KEY", "Valid Name", "Valid Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenKeyIsInvalid()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateProjectCommand("", "Valid Name", "Valid Description");

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
            .Setup(u => u.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateProjectCommand("VALID-KEY", new string('A', 101), "Valid Description");

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
            .Setup(u => u.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateProjectCommand("VALID-KEY", "Valid Name", new string('A', 501));

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
        _projectRepositoryMock
            .Setup(u => u.ExistsByKeyAsync("DUPLICATE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateProjectCommand("DUPLICATE", "Valid Name", "Valid Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Key");
    }
}
