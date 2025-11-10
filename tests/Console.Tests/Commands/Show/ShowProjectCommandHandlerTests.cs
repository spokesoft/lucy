using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;
using Lucy.Application.Projects.Queries.GetProjectById;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Console.Commands.Show;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.Show;

public class ShowProjectCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IViewRenderer<ProjectDto>> _viewRendererMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ShowProjectCommandHandler _handler;

    public ShowProjectCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _viewRendererMock = new Mock<IViewRenderer<ProjectDto>>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new ShowProjectCommandHandler(
            _console,
            _localizerMock.Object,
            _viewRendererMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidId_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new ShowProjectCommand { Id = projectId, Key = null };
        var project = new ProjectDto
        {
            Id = projectId,
            Key = "TEST",
            Name = "Test Project",
            Description = "Test Description",
            CreatedAt = DateTime.Parse("2025-11-05 19:08:25Z").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-11-05 19:08:25Z").ToUniversalTime()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                project,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithValidKey_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var projectKey = "TEST";
        var command = new ShowProjectCommand { Id = null, Key = projectKey };
        var project = new ProjectDto
        {
            Id = projectId,
            Key = projectKey,
            Name = "Test Project",
            Description = "Test Description",
            CreatedAt = DateTime.Parse("2025-11-05 19:08:25Z").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-11-05 19:08:25Z").ToUniversalTime()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProjectIdByKeyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                project,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ProjectNotFound_ReturnsError()
    {
        // Arrange
        var projectId = 1L;
        var command = new ShowProjectCommand { Id = projectId, Key = null };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectDto?)null);
        _localizerMock
            .Setup(l => l["Error.Project.NotFound"])
            .Returns(new LocalizedString("Error.Project.NotFound", "Project not found"));

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Error, result);
        Assert.Contains("Project not found", _console.Output);
    }
}
