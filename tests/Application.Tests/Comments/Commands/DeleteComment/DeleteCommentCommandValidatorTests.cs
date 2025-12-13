using Lucy.Application.Comments.Commands.DeleteComment;
using Lucy.Application.Comments.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Comments.Commands.DeleteComment;

public class DeleteCommentCommandValidatorTests : ApplicationTestBase
{
    private readonly Mock<ICommentReadOnlyRepository> _commentRepositoryMock;
    private readonly DeleteCommentCommandValidator _validator;

    public DeleteCommentCommandValidatorTests()
    {
        _commentRepositoryMock = SetupReadOnlyRepository(u => u.Comments);
        _validator = new DeleteCommentCommandValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldValidate()
    {
        // Arrange
        _commentRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new DeleteCommentCommand(1);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenCommentDoesNotExist()
    {
        // Arrange
        _commentRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new DeleteCommentCommand(1);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }
}
