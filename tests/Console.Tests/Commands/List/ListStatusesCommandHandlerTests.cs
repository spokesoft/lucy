using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Statuses.Queries.ListStatuses;
using Lucy.Console.Commands.List;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Lucy.Domain.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.List;

public class ListStatusesCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IViewRenderer<IEnumerable<StatusDto>>> _viewRendererMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ListStatusesCommandHandler _handler;

    public ListStatusesCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _viewRendererMock = new Mock<IViewRenderer<IEnumerable<StatusDto>>>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new ListStatusesCommandHandler(
            _console,
            _localizerMock.Object,
            _viewRendererMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithProjectKeyAndStatuses_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var projectKey = "EXAMP";
        var command = new ListStatusesCommand
        {
            Key = projectKey,
            Id = null
        };
        var statuses = new List<StatusDto>
        {
            new()
            {
                Id = 1,
                ProjectId = projectId,
                Key = "TODO",
                Order = 1,
                Name = "To Do",
                Description = "Tasks to do",
                Color = StatusColor.Gray,
                CreatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime(),
                UpdatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime()
            },
            new()
            {
                Id = 2,
                ProjectId = projectId,
                Key = "IN-PROGRESS",
                Order = 2,
                Name = "In Progress",
                Description = "Work in progress",
                Color = StatusColor.Blue,
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
                It.Is<ListStatusesQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                statuses,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithProjectIdAndStatuses_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new ListStatusesCommand
        {
            Key = null,
            Id = projectId
        };
        var statuses = new List<StatusDto>
        {
            new()
            {
                Id = 1,
                ProjectId = projectId,
                Key = "TODO",
                Order = 1,
                Name = "To Do",
                Description = "Tasks to do",
                Color = StatusColor.Gray,
                CreatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime(),
                UpdatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime()
            }
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListStatusesQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                statuses,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NoStatuses_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new ListStatusesCommand
        {
            Key = null,
            Id = projectId
        };
        var statuses = new List<StatusDto>();

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListStatusesQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                statuses,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
