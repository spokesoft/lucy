using Lucy.Application.Comments.DTOs;
using Lucy.Application.Comments.Queries.ListTicketComments;
using Lucy.Application.Comments.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Comments.Queries.ListTicketComments;

public class ListTicketCommentsQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<ICommentReadOnlyRepository> _commentRepositoryMock;
    private readonly ListTicketCommentsQueryHandler _handler;

    public ListTicketCommentsQueryHandlerTests()
    {
        _commentRepositoryMock = SetupReadOnlyRepository(u => u.Comments);
        _handler = new ListTicketCommentsQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnComments()
    {
        // Arrange
        var comments = new List<TicketCommentDto>
        {
            new TicketCommentDto { Id = 1, Content = "Content 1" },
            new TicketCommentDto { Id = 2, Content = "Content 2" }
        };

        _commentRepositoryMock.Setup(
            repo => repo.GetTicketCommentsAsync(1, CancellationToken.None)).ReturnsAsync(comments);

        var query = new ListTicketCommentsQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}
