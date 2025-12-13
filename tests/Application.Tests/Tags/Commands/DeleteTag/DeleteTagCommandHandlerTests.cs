using Lucy.Application.Tags.Commands.DeleteTag;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tags.Commands.DeleteTag;

public class DeleteTagCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly DeleteTagCommandHandler _handler;

    public DeleteTagCommandHandlerTests()
    {
        _tagRepositoryMock = SetupRepository(u => u.Tags);
        _handler = new DeleteTagCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteTag_WhenValidCommandIsGiven()
    {
        // Arrange
        var tag = new Tag(1, "test-key", "Test Tag", "Test Description", Color.Blue);
        tag.Id = 1;

        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        _tagRepositoryMock
            .Setup(repo => repo.Remove(It.IsAny<Tag>()));

        var command = new DeleteTagCommand(1);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _tagRepositoryMock.Verify(repo => repo.Remove(tag), Times.Once);
        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}
