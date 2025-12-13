using Lucy.Application.Projects.Commands.UpdateProject;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Projects.Commands.UpdateProject;

public class UpdateProjectCommandValidatorTests : ApplicationTestBase
{
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly UpdateProjectCommandValidator _validator;

    public UpdateProjectCommandValidatorTests()
    {
        _projectRepositoryMock = SetupReadOnlyRepository(u => u.Projects);
        _validator = new UpdateProjectCommandValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldValidate()
    {
        // Arrange
        var existingProject = new Project("OLD-KEY", "Old Name", "Old Description");
        existingProject.Id = 1;

        _projectRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProject);

        _projectRepositoryMock
            .Setup(u => u.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new UpdateProjectCommand(1, "VALID-KEY", "Valid Name", "Valid Description");

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
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var command = new UpdateProjectCommand(1, "VALID-KEY", "Valid Name", "Valid Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenKeyIsInvalid()
    {
        // Arrange
        var existingProject = new Project("OLD-KEY", "Old Name", "Old Description");
        existingProject.Id = 1;

        _projectRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProject);

        var command1 = new UpdateProjectCommand(1, string.Empty, "Valid Name", "Valid Description");
        var command2 = new UpdateProjectCommand(1, " ", "Valid Name", "Valid Description");
        var command3 = new UpdateProjectCommand(1, "1ABC", "Valid Name", "Valid Description");
        var command4 = new UpdateProjectCommand(1, "KEYISTOOLONG", "Valid Name", "Valid Description");

        // Act
        var result1 = await _validator.ValidateAsync(command1);
        var result2 = await _validator.ValidateAsync(command2);
        var result3 = await _validator.ValidateAsync(command3);
        var result4 = await _validator.ValidateAsync(command4);

        // Assert
        Assert.False(result1.IsValid);
        Assert.Contains(result1.Errors, e => e.PropertyName == "Key");

        Assert.False(result2.IsValid);
        Assert.Contains(result2.Errors, e => e.PropertyName == "Key");

        Assert.False(result3.IsValid);
        Assert.Contains(result3.Errors, e => e.PropertyName == "Key");

        Assert.False(result4.IsValid);
        Assert.Contains(result4.Errors, e => e.PropertyName == "Key");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenKeyIsNotUnique()
    {
        // Arrange
        var existingProject = new Project("OLD-KEY", "Old Name", "Old Description");
        existingProject.Id = 1;

        _projectRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProject);

        _projectRepositoryMock
            .Setup(u => u.ExistsByKeyAsync("DUPLICATE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new UpdateProjectCommand(1, "DUPLICATE", "Valid Name", "Valid Description");

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
        var existingProject = new Project("OLD-KEY", "Old Name", "Old Description");
        existingProject.Id = 1;

        _projectRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProject);

        _projectRepositoryMock
            .Setup(u => u.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new UpdateProjectCommand(1, "VALID-KEY", new string('A', 101), "Valid Description");

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
        var existingProject = new Project("OLD-KEY", "Old Name", "Old Description");
        existingProject.Id = 1;

        _projectRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProject);

        _projectRepositoryMock
            .Setup(u => u.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new UpdateProjectCommand(1, "VALID-KEY", "Valid Name", new string('A', 501));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }
}
