using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Console.Commands.Delete;
using Lucy.Console.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.Delete;

public class DeleteStatusCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DeleteStatusCommandHandler _handler;

    public DeleteStatusCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _mediatorMock = new Mock<IMediator>();

        // Setup localizer to return localized strings
        _localizerMock.Setup(x => x[It.IsAny<string>(), It.IsAny<object[]>()])
            .Returns((string key, object[] args) => new LocalizedString(key, string.Format(key, args)));

        _handler = new DeleteStatusCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_DeleteByKeysWithProjectKey_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 5L;
        var projectKey = "EXAMP";
        var statusKey = "TODO";
        var command = new DeleteStatusCommand
        {
            ProjectKey = projectKey,
            StatusKey = statusKey,
            ProjectId = null,
            StatusId = null
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == projectKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetStatusByKeyQuery>(q => q.ProjectId == projectId && q.Key == statusKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StatusDto { Id = statusId });

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.DeletedStatus", _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<Application.Statuses.Commands.DeleteStatus.DeleteStatusCommand>(c => c.Id == statusId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeleteByKeysWithProjectId_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 5L;
        var statusKey = "TODO";
        var command = new DeleteStatusCommand
        {
            ProjectKey = null,
            StatusKey = statusKey,
            ProjectId = projectId,
            StatusId = null
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetStatusByKeyQuery>(q => q.ProjectId == projectId && q.Key == statusKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StatusDto { Id = statusId });

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.DeletedStatus", _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<Application.Statuses.Commands.DeleteStatus.DeleteStatusCommand>(c => c.Id == statusId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeleteById_ReturnsSuccess()
    {
        // Arrange
        var statusId = 5L;
        var command = new DeleteStatusCommand
        {
            ProjectKey = null,
            StatusKey = null,
            ProjectId = null,
            StatusId = statusId
        };

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.DeletedStatus", _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<Application.Statuses.Commands.DeleteStatus.DeleteStatusCommand>(c => c.Id == statusId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
