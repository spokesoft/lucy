using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.DTOs;
using Lucy.Application.Projects.Queries.ListProjects;
using Lucy.Console.Commands.List;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.List;

public class ListProjectsCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IViewRenderer<IEnumerable<ProjectDto>>> _viewRendererMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ListProjectsCommandHandler _handler;

    public ListProjectsCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _viewRendererMock = new Mock<IViewRenderer<IEnumerable<ProjectDto>>>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new ListProjectsCommandHandler(
            _console,
            _localizerMock.Object,
            _viewRendererMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithProjects_ReturnsSuccess()
    {
        // Arrange
        var command = new ListProjectsCommand();
        var projects = new List<ProjectDto>
        {
            new()
            {
                Id = 1,
                Key = "TEST1",
                Name = "Test Project 1",
                Description = "Test Description 1",
                CreatedAt = DateTime.Parse("2025-11-05 19:15:32Z").ToUniversalTime(),
                UpdatedAt = DateTime.Parse("2025-11-05 19:15:32Z").ToUniversalTime()
            },
            new()
            {
                Id = 2,
                Key = "TEST2",
                Name = "Test Project 2",
                Description = "Test Description 2",
                CreatedAt = DateTime.Parse("2025-11-05 19:15:32Z").ToUniversalTime(),
                UpdatedAt = DateTime.Parse("2025-11-05 19:15:32Z").ToUniversalTime()
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ListProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                projects,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NoProjects_ReturnsSuccess()
    {
        // Arrange
        var command = new ListProjectsCommand();
        var projects = new List<ProjectDto>();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<ListProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                projects,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
