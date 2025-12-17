using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tags.DTOs;
using Lucy.Application.Tags.Queries.ListTags;
using Lucy.Console.Commands.List;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Lucy.Domain.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.List;

public class ListTagsCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IViewRenderer<IEnumerable<TagDto>>> _viewRendererMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ListTagsCommandHandler _handler;

    public ListTagsCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _viewRendererMock = new Mock<IViewRenderer<IEnumerable<TagDto>>>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new ListTagsCommandHandler(
            _console,
            _localizerMock.Object,
            _viewRendererMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithProjectKeyAndTags_ReturnsSuccess()
    {
        var projectId = 1L;
        var projectKey = "EXAMP";
        var command = new ListTagsCommand
        {
            Key = projectKey,
            Id = null
        };
        var tags = new List<TagDto>
        {
            new()
            {
                Id = 1,
                ProjectId = projectId,
                Key = "BUG",
                Label = "Bug",
                Description = "Bug related",
                Color = Color.Red,
                CreatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime(),
                UpdatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime()
            },
            new()
            {
                Id = 2,
                ProjectId = projectId,
                Key = "DOCS",
                Label = "Docs",
                Description = "Documentation",
                Color = Color.Blue,
                CreatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime(),
                UpdatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime()
            }
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == projectKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListTagsQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                tags,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithProjectIdAndTags_ReturnsSuccess()
    {
        var projectId = 1L;
        var command = new ListTagsCommand
        {
            Key = null,
            Id = projectId
        };
        var tags = new List<TagDto>
        {
            new()
            {
                Id = 1,
                ProjectId = projectId,
                Key = "BUG",
                Label = "Bug",
                Description = "Bug related",
                Color = Color.Red,
                CreatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime(),
                UpdatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime()
            }
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListTagsQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                tags,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NoTags_ReturnsSuccess()
    {
        var projectId = 1L;
        var command = new ListTagsCommand
        {
            Key = null,
            Id = projectId
        };
        var tags = new List<TagDto>();

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListTagsQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                tags,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
