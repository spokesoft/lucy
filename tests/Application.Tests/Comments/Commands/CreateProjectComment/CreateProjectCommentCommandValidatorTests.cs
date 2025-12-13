using Lucy.Application.Comments.Commands.CreateProjectComment;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Comments.Commands.CreateProjectComment;

public class CreateProjectCommentCommandValidatorTests : ApplicationTestBase
{
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly CreateProjectCommentCommandValidator _validator;

    public CreateProjectCommentCommandValidatorTests()
    {
        _projectRepositoryMock = SetupReadOnlyRepository(u => u.Projects);
        _validator = new CreateProjectCommentCommandValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldValidate()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateProjectCommentCommand(1, "Valid comment content");

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

        var command = new CreateProjectCommentCommand(1, "Valid comment content");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProjectId");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenContentIsEmpty()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateProjectCommentCommand(1, string.Empty);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Content");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenContentIsTooLong()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateProjectCommentCommand(1, new string('A', 5001));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Content");
    }
}
