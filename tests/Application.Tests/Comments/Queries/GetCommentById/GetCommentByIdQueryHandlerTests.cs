using Lucy.Application.Comments.DTOs;
using Lucy.Application.Comments.Queries.GetCommentById;
using Lucy.Application.Comments.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Comments.Queries.GetCommentById;

public class GetCommentByIdQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<ICommentReadOnlyRepository> _commentRepositoryMock;
    private readonly GetCommentByIdQueryHandler _handler;

    public GetCommentByIdQueryHandlerTests()
    {
        _commentRepositoryMock = SetupReadOnlyRepository(u => u.Comments);
        _handler = new GetCommentByIdQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnComment_WhenCommentExists()
    {
        // Arrange
        var comment = new Domain.Entities.ProjectComment(1, "Content");
        comment.Id = 1;

        _commentRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(comment);

        var query = new GetCommentByIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Content", result.Content);
        Assert.IsType<ProjectCommentDto>(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenCommentDoesNotExist()
    {
        // Arrange
        _commentRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync((Domain.Entities.Comment)null!);

        var query = new GetCommentByIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
