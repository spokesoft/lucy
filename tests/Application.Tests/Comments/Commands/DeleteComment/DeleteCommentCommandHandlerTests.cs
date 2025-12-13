using Lucy.Application.Comments.Commands.DeleteComment;
using Lucy.Application.Comments.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Comments.Commands.DeleteComment;

public class DeleteCommentCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly DeleteCommentCommandHandler _handler;

    public DeleteCommentCommandHandlerTests()
    {
        _commentRepositoryMock = SetupRepository(u => u.Comments);
        _handler = new DeleteCommentCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteComment_WhenCommentExists()
    {
        // Arrange
        var comment = new ProjectComment(1, "Content");
        comment.Id = 1;

        _commentRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(comment);

        var command = new DeleteCommentCommand(1);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _commentRepositoryMock.Verify(repo => repo.Remove(comment), Times.Once);
        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenCommentDoesNotExist()
    {
        // Arrange
        _commentRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync((Comment)null!);

        var command = new DeleteCommentCommand(1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command, CancellationToken.None));
    }
}
