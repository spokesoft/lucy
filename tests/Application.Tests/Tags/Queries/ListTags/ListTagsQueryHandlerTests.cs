using Lucy.Application.Tags.Queries.ListTags;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tags.Queries.ListTags;

public class ListTagsQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITagReadOnlyRepository> _tagRepositoryMock;
    private readonly ListTagsQueryHandler _handler;

    public ListTagsQueryHandlerTests()
    {
        _tagRepositoryMock = SetupReadOnlyRepository(u => u.Tags);
        _handler = new ListTagsQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllTags()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new(1, "BUG", "Bug", "Bug reports", Color.Red),
            new(1, "FEATURE", "Feature", "New features", Color.Blue),
            new(1, "ENHANCEMENT", "Enhancement", "Improvements", Color.Green)
        };

        _tagRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        var query = new ListTagsQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("BUG", result[0].Key);
        Assert.Equal("FEATURE", result[1].Key);
        Assert.Equal("ENHANCEMENT", result[2].Key);
    }
}
