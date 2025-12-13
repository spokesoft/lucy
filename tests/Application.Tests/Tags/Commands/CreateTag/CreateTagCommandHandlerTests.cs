using Lucy.Application.Tags.Commands.CreateTag;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tags.Commands.CreateTag;

public class CreateTagCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly CreateTagCommandHandler _handler;

    public CreateTagCommandHandlerTests()
    {
        _tagRepositoryMock = SetupRepository(u => u.Tags);
        SetupRepository(u => u.Projects); // Needed if handler checks project, but usually validator does.
        _handler = new CreateTagCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateTag_WhenValidCommandIsGiven()
    {
        // Arrange
        _tagRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()))
            .Callback<Tag, CancellationToken>((tag, _) => tag.Id = 1)
            .Returns(Task.CompletedTask);

        var command = new CreateTagCommand(1, "test-key", "Test Tag", "Test Description", Color.Blue);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
        _tagRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Tag>(t => t.ProjectId == 1 && t.Key == "TEST-KEY" && t.Label == "Test Tag" && t.Description == "Test Description" && t.Color == Color.Blue), It.IsAny<CancellationToken>()), Times.Once);
        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}
