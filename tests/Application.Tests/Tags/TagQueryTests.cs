using Lucy.Application.Interfaces;
using Lucy.Application.Queries;
using Lucy.Application.Tags.DTOs;
using Lucy.Application.Tags.Queries.GetTagById;
using Lucy.Application.Tags.Queries.ListTags;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;

namespace Lucy.Application.Tests.Tags;

public class TagQueryTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;

    public TagQueryTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
    }

    [Fact]
    public async Task ListTagsQueryHandler_ShouldReturnAllTags()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new Tag(1, "BUG", "Bug", "Bug reports", Color.Red),
            new Tag(1, "FEATURE", "Feature", "New features", Color.Blue),
            new Tag(1, "ENHANCEMENT", "Enhancement", "Improvements", Color.Green)
        };

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tags.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        var handler = new ListTagsQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListTagsQuery();

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("BUG", result[0].Key);
        Assert.Equal("FEATURE", result[1].Key);
        Assert.Equal("ENHANCEMENT", result[2].Key);
    }

    [Fact]
    public async Task GetTagByIdQueryHandler_ShouldReturnTag_WhenTagExists()
    {
        // Arrange
        var tag = new Tag(1, "BUG", "Bug", "Bug reports", Color.Red);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tags.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var handler = new GetTagByIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetTagByIdQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("BUG", result.Key);
        Assert.Equal("Bug", result.Label);
        Assert.Equal("Bug reports", result.Description);
        Assert.Equal(Color.Red, result.Color);
    }

    [Fact]
    public async Task GetTagByIdQueryHandler_ShouldReturnNull_WhenTagDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tags.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag)null!);

        var handler = new GetTagByIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetTagByIdQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
