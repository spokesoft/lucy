using Lucy.Application.Projects.Repositories;
using Lucy.Application.Tags.Commands.CreateTag;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Common.Validation;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tags.Commands.CreateTag;

public class CreateTagCommandValidatorTests : ApplicationTestBase
{
    private readonly Mock<ITagReadOnlyRepository> _tagRepositoryMock;
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly CreateTagCommandValidator _validator;

    public CreateTagCommandValidatorTests()
    {
        _tagRepositoryMock = SetupReadOnlyRepository(u => u.Tags);
        _projectRepositoryMock = SetupReadOnlyRepository(u => u.Projects);
        _validator = new CreateTagCommandValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnSuccess_WhenValidCommandIsGiven()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(repo => repo.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _tagRepositoryMock
            .Setup(repo => repo.ExistsByKeyAsync(1, "TEST-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateTagCommand(1, "test-key", "Test Tag", "Test Description", Color.Green);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnError_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(repo => repo.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateTagCommand(1, "test-key", "Test Tag", "Test Description", null);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.ProjectNotFound.ToString());
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnError_WhenKeyAlreadyExists()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(repo => repo.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _tagRepositoryMock
            .Setup(repo => repo.ExistsByKeyAsync(1, "test-key", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateTagCommand(1, "test-key", "Test Tag", "Test Description", Color.Red);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TagKeyExists.ToString());
    }
}
