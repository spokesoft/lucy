using Lucy.Application.Comments.Commands.UpdateComment;
using Lucy.Application.Comments.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Comments.Commands.UpdateComment;

public class UpdateCommentCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly UpdateCommentCommandHandler _handler;

    public UpdateCommentCommandHandlerTests()
    {
        _commentRepositoryMock = SetupRepository(u => u.Comments);
        _handler = new UpdateCommentCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateComment_WhenCommentExists()
    {
        // Arrange
        var comment = new ProjectComment(1, "Old Content");
        comment.Id = 1;

        _commentRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(comment);

        var command = new UpdateCommentCommand(1, "New Content");

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal("New Content", comment.Content);
        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenCommentDoesNotExist()
    {
        // Arrange
        _commentRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync((Comment)null!);

        var command = new UpdateCommentCommand(1, "New Content");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command, CancellationToken.None));
    }
}
