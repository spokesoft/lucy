using Lucy.Application.Interfaces;
using Lucy.Application.TicketTags.Queries.ListTagsByTicketId;
using Lucy.Application.TicketTags.Repositories;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;

namespace Lucy.Application.Tests.TicketTags.Queries.ListTagsByTicketId;

public class ListTagsByTicketIdQueryHandlerTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<ITicketTagReadOnlyRepository> _ticketTagRepositoryMock;
    private readonly ListTagsByTicketIdQueryHandler _handler;

    public ListTagsByTicketIdQueryHandlerTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _ticketTagRepositoryMock = new Mock<ITicketTagReadOnlyRepository>();

        _readOnlyUnitOfWorkMock.Setup(u => u.TicketTags).Returns(_ticketTagRepositoryMock.Object);

        _handler = new ListTagsByTicketIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnMappedTags()
    {
        // Arrange
        var createdAt = new DateTime(2024, 1, 1);
        var updatedAt = new DateTime(2024, 1, 2);

        var tags = new List<Tag>
        {
            new Tag(1, "BUG", "Bug", "Bugs", Color.Red)
            {
                Id = 5,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            },
            new Tag(1, "FEAT", "Feature", null, Color.Green)
            {
                Id = 6,
                CreatedAt = createdAt.AddDays(1),
                UpdatedAt = updatedAt.AddDays(1)
            }
        };

        _ticketTagRepositoryMock
            .Setup(u => u.GetTagsByTicketIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        var query = new ListTagsByTicketIdQuery(10);

        // Act
        var result = (await _handler.HandleAsync(query, CancellationToken.None)).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Collection(result,
            tagDto =>
            {
                Assert.Equal(5, tagDto.Id);
                Assert.Equal(1, tagDto.ProjectId);
                Assert.Equal("BUG", tagDto.Key);
                Assert.Equal("Bug", tagDto.Label);
                Assert.Equal("Bugs", tagDto.Description);
                Assert.Equal(Color.Red, tagDto.Color);
                Assert.Equal(createdAt, tagDto.CreatedAt);
                Assert.Equal(updatedAt, tagDto.UpdatedAt);
            },
            tagDto =>
            {
                Assert.Equal(6, tagDto.Id);
                Assert.Equal(1, tagDto.ProjectId);
                Assert.Equal("FEAT", tagDto.Key);
                Assert.Equal("Feature", tagDto.Label);
                Assert.Null(tagDto.Description);
                Assert.Equal(Color.Green, tagDto.Color);
                Assert.Equal(createdAt.AddDays(1), tagDto.CreatedAt);
                Assert.Equal(updatedAt.AddDays(1), tagDto.UpdatedAt);
            });

        _ticketTagRepositoryMock.Verify(
            u => u.GetTagsByTicketIdAsync(10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenNoTagsFound()
    {
        // Arrange
        _ticketTagRepositoryMock
            .Setup(u => u.GetTagsByTicketIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tag>());

        var query = new ListTagsByTicketIdQuery(10);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _ticketTagRepositoryMock.Verify(
            u => u.GetTagsByTicketIdAsync(10, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
