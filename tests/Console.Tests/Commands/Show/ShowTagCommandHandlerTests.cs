using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;
using Lucy.Application.Projects.Queries.GetProjectById;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tags.DTOs;
using Lucy.Application.Tags.Queries.GetTagById;
using Lucy.Application.Tags.Queries.GetTagIdByKey;
using Lucy.Console.Commands.Show;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.Show;

public class ShowTagCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IViewRenderer<(TagDto, ProjectDto)>> _viewRendererMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ShowTagCommandHandler _handler;

    public ShowTagCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _viewRendererMock = new Mock<IViewRenderer<(TagDto, ProjectDto)>>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new ShowTagCommandHandler(
            _console,
            _localizerMock.Object,
            _viewRendererMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidId_ReturnsSuccess()
    {
        // Arrange
        var tagId = 5L;
        var command = new ShowTagCommand
        {
            TagId = tagId,
            TagKey = null,
            ProjectKey = null,
            ProjectId = null
        };
        var tag = new TagDto
        {
            Id = tagId,
            ProjectId = 1L,
            Key = "BUG",
            Label = "Bug",
            Description = "Bug tag",
            Color = Lucy.Domain.Enums.Color.Red,
            CreatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTagByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var dummyProject = new ProjectDto
        {
            Id = tag.ProjectId,
            Key = "EXAMP",
            Name = "Example Project",
            Description = "Project Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dummyProject);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                It.Is<(TagDto, ProjectDto)>(t =>
                    t.Item1.Id == tag.Id &&
                    t.Item2.Id == dummyProject.Id),
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithValidKeysUsingProjectKey_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var tagId = 5L;
        var projectKey = "EXAMP";
        var tagKey = "BUG";

        var command = new ShowTagCommand
        {
            TagId = null,
            TagKey = tagKey,
            ProjectKey = projectKey,
            ProjectId = null
        };

        var tagDto = new TagDto
        {
            Id = tagId,
            ProjectId = projectId,
            Key = tagKey,
            Label = "Bug",
            Description = "Bug tag",
            Color = Lucy.Domain.Enums.Color.Red,
            CreatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProjectIdByKeyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTagIdByKeyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tagId);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTagByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tagDto);

        var dummyProject = new ProjectDto
        {
            Id = projectId,
            Key = projectKey,
            Name = "Example Project",
            Description = "Project Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dummyProject);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _viewRendererMock.Verify(
            v => v.RenderAsync(
                It.Is<(TagDto, ProjectDto)>(t =>
                    t.Item1.Id == tagDto.Id &&
                    t.Item2.Id == dummyProject.Id),
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithValidKeysUsingProjectId_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var tagId = 5L;
        var tagKey = "BUG";

        var command = new ShowTagCommand
        {
            TagId = null,
            TagKey = tagKey,
            ProjectKey = null,
            ProjectId = projectId
        };

        var tagDto = new TagDto
        {
            Id = tagId,
            ProjectId = projectId,
            Key = tagKey,
            Label = "Bug",
            Description = "Bug tag",
            Color = Lucy.Domain.Enums.Color.Red,
            CreatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTagIdByKeyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tagId);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTagByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tagDto);

        var dummyProject = new ProjectDto
        {
            Id = projectId,
            Key = "EXAMP",
            Name = "Example Project",
            Description = "Project Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dummyProject);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _viewRendererMock.Verify(
            v => v.RenderAsync(
                It.Is<(TagDto, ProjectDto)>(t =>
                    t.Item1.Id == tagDto.Id &&
                    t.Item2.Id == dummyProject.Id),
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_TagNotFound_ReturnsError()
    {
        // Arrange
        var tagId = 5L;
        var command = new ShowTagCommand
        {
            TagId = tagId,
            TagKey = null,
            ProjectKey = null,
            ProjectId = null
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTagByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TagDto?)null);

        _localizerMock
            .Setup(l => l["Error.Tag.NotFound"])
            .Returns(new LocalizedString("Error.Tag.NotFound", "Tag not found"));

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Error, result);
        Assert.Contains("Tag not found", _console.Output);
    }
}
