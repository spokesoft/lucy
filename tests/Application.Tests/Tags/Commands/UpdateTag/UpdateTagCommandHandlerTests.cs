using Lucy.Application.Tags.Commands.UpdateTag;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tags.Commands.UpdateTag;

public class UpdateTagCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly UpdateTagCommandHandler _handler;

    public UpdateTagCommandHandlerTests()
    {
        _tagRepositoryMock = SetupRepository(u => u.Tags);
        _handler = new UpdateTagCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateTag_WhenValidCommandIsGiven()
    {
        // Arrange
        var tag = new Tag(1, "old-key", "Old Tag", "Old Description", Color.Blue);
        tag.Id = 1;

        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var command = new UpdateTagCommand(1, "new-key", "New Tag", "New Description", Color.Red);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal("NEW-KEY", tag.Key);
        Assert.Equal("New Tag", tag.Label);
        Assert.Equal("New Description", tag.Description);
        Assert.Equal(Color.Red, tag.Color);
        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}
