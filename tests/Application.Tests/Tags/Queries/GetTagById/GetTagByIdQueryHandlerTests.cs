using Lucy.Application.Tags.Queries.GetTagById;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tags.Queries.GetTagById;

public class GetTagByIdQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITagReadOnlyRepository> _tagRepositoryMock;
    private readonly GetTagByIdQueryHandler _handler;

    public GetTagByIdQueryHandlerTests()
    {
        _tagRepositoryMock = SetupReadOnlyRepository(u => u.Tags);
        _handler = new GetTagByIdQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTag_WhenTagExists()
    {
        // Arrange
        var tag = new Tag(1, "BUG", "Bug", "Bug reports", Color.Red);

        _tagRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var query = new GetTagByIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("BUG", result.Key);
        Assert.Equal("Bug", result.Label);
        Assert.Equal("Bug reports", result.Description);
        Assert.Equal(Color.Red, result.Color);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenTagDoesNotExist()
    {
        // Arrange
        _tagRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag)null!);

        var query = new GetTagByIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
