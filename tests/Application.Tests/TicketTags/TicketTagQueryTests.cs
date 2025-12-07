using System.Linq;
using Lucy.Application.Interfaces;
using Lucy.Application.TicketTags.Queries.ListTagsByTicketId;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;

namespace Lucy.Application.Tests.TicketTags;

public class TicketTagQueryTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;

    public TicketTagQueryTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
    }

    [Fact]
    public async Task ListTagsByTicketIdQueryHandler_ShouldReturnMappedTags()
    {
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

        _readOnlyUnitOfWorkMock
            .Setup(u => u.TicketTags.GetTagsByTicketIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        var handler = new ListTagsByTicketIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListTagsByTicketIdQuery(10);

        var result = (await handler.HandleAsync(query, CancellationToken.None)).ToList();

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

        _readOnlyUnitOfWorkMock.Verify(
            u => u.TicketTags.GetTagsByTicketIdAsync(10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ListTagsByTicketIdQueryHandler_ShouldReturnEmptyList_WhenNoTagsFound()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.TicketTags.GetTagsByTicketIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tag>());

        var handler = new ListTagsByTicketIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListTagsByTicketIdQuery(10);

        var result = await handler.HandleAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);

        _readOnlyUnitOfWorkMock.Verify(
            u => u.TicketTags.GetTagsByTicketIdAsync(10, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
