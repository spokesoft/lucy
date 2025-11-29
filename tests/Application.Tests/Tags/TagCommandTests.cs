using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Tags.Commands.CreateTag;
using Lucy.Application.Tags.Commands.DeleteTag;
using Lucy.Application.Tags.Commands.UpdateTag;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tickets.Repositories;
using Lucy.Application.Validation;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;

namespace Lucy.Application.Tests.Tags;

public class TagCommandTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;

    public TagCommandTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _tagRepositoryMock = new Mock<ITagRepository>();
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();

        _unitOfWorkMock.Setup(u => u.Tags).Returns(_tagRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Projects).Returns(_projectRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Tickets).Returns(_ticketRepositoryMock.Object);

        _readOnlyUnitOfWorkMock.Setup(u => u.Tags).Returns(_tagRepositoryMock.As<ITagReadOnlyRepository>().Object);
        _readOnlyUnitOfWorkMock.Setup(u => u.Projects).Returns(_projectRepositoryMock.As<IProjectReadOnlyRepository>().Object);
        _readOnlyUnitOfWorkMock.Setup(u => u.Tickets).Returns(_ticketRepositoryMock.As<ITicketReadOnlyRepository>().Object);
    }

    [Fact]
    public async Task CreateTagCommandHandler_ShouldCreateTag_WhenValidCommandIsGiven()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");

        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _tagRepositoryMock
            .Setup(repo => repo.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tag>());

        _tagRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()))
            .Callback<Tag, CancellationToken>((tag, _) => tag.Id = 1)
            .Returns(Task.CompletedTask);

        var handler = new CreateTagCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateTagCommand(1, "test-key", "Test Tag", "Test Description");

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);
        _tagRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Tag>(t => t.ProjectId == 1 && t.Key == "TEST-KEY" && t.Label == "Test Tag" && t.Description == "Test Description"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTagCommandValidator_ShouldReturnSuccess_WhenValidCommandIsGiven()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");

        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _projectRepositoryMock
            .Setup(repo => repo.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _tagRepositoryMock
            .Setup(repo => repo.ExistsByKeyAsync(1, "TEST-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new CreateTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateTagCommand(1, "test-key", "Test Tag", "Test Description");

        // Act
        var result = await validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreateTagCommandValidator_ShouldReturnError_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project)null!);

        var validator = new CreateTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateTagCommand(1, "test-key", "Test Tag", "Test Description");

        // Act
        var result = await validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.ProjectNotFound.ToString());
    }

    [Fact]
    public async Task CreateTagCommandValidator_ShouldReturnError_WhenKeyAlreadyExists()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");

        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _projectRepositoryMock
            .Setup(repo => repo.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _tagRepositoryMock
            .Setup(repo => repo.ExistsByKeyAsync(1, "test-key", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new CreateTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateTagCommand(1, "test-key", "Test Tag", "Test Description");

        // Act
        var result = await validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TagKeyExists.ToString());
    }

    [Fact]
    public async Task UpdateTagCommandHandler_ShouldUpdateTag_WhenValidCommandIsGiven()
    {
        // Arrange
        var tag = new Tag(1, "old-key", "Old Tag", "Old Description");

        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var handler = new UpdateTagCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateTagCommand(1, "new-key", "New Tag", "New Description");

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal("NEW-KEY", tag.Key);
        Assert.Equal("New Tag", tag.Label);
        Assert.Equal("New Description", tag.Description);
    }

    [Fact]
    public async Task UpdateTagCommandValidator_ShouldReturnSuccess_WhenValidCommandIsGiven()
    {
        // Arrange
        var tag = new Tag(1, "old-key", "Old Tag", "Old Description");

        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        _tagRepositoryMock
            .Setup(repo => repo.ExistsByKeyAsync(1, "NEW-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new UpdateTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateTagCommand(1, "new-key", "New Tag", "New Description");

        // Act
        var result = await validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UpdateTagCommandValidator_ShouldReturnError_WhenTagDoesNotExist()
    {
        // Arrange
        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag)null!);

        var validator = new UpdateTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateTagCommand(1, "new-key", "New Tag", "New Description");

        // Act
        var result = await validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TagNotFound.ToString());
    }

    [Fact]
    public async Task UpdateTagCommandValidator_ShouldReturnError_WhenKeyAlreadyExists()
    {
        // Arrange
        var tag = new Tag(1, "old-key", "Old Tag", "Old Description");

        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        _tagRepositoryMock
            .Setup(repo => repo.ExistsByKeyAsync(1, "new-key", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new UpdateTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateTagCommand(1, "new-key", "New Tag", "New Description");

        // Act
        var result = await validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TagKeyExists.ToString());
    }

    [Fact]
    public async Task DeleteTagCommandHandler_ShouldDeleteTag_WhenValidCommandIsGiven()
    {
        // Arrange
        var tag = new Tag(1, "test-key", "Test Tag", "Test Description");

        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        _tagRepositoryMock
            .Setup(repo => repo.Remove(It.IsAny<Tag>()));

        var handler = new DeleteTagCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteTagCommand(1);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _tagRepositoryMock.Verify(repo => repo.Remove(tag), Times.Once);
    }

    [Fact]
    public async Task DeleteTagCommandValidator_ShouldReturnSuccess_WhenTagExists()
    {
        // Arrange
        _tagRepositoryMock
            .Setup(repo => repo.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new DeleteTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new DeleteTagCommand(1);

        // Act
        var result = await validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task DeleteTagCommandValidator_ShouldReturnError_WhenTagDoesNotExist()
    {
        // Arrange
        _tagRepositoryMock
            .Setup(repo => repo.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new DeleteTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new DeleteTagCommand(1);

        // Act
        var result = await validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TagNotFound.ToString());
    }
}
