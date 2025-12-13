using Lucy.Application.Comments.DTOs;
using Lucy.Application.Comments.Queries.ListProjectComments;
using Lucy.Application.Comments.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Comments.Queries.ListProjectComments;

public class ListProjectCommentsQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<ICommentReadOnlyRepository> _commentRepositoryMock;
    private readonly ListProjectCommentsQueryHandler _handler;

    public ListProjectCommentsQueryHandlerTests()
    {
        _commentRepositoryMock = SetupReadOnlyRepository(u => u.Comments);
        _handler = new ListProjectCommentsQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnComments()
    {
        // Arrange
        var comments = new List<ProjectCommentDto>
        {
            new ProjectCommentDto { Id = 1, Content = "Content 1" },
            new ProjectCommentDto { Id = 2, Content = "Content 2" }
        };

        _commentRepositoryMock.Setup(
            repo => repo.GetProjectCommentsAsync(1, CancellationToken.None)).ReturnsAsync(comments);

        var query = new ListProjectCommentsQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}
