using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tags.Queries.GetTagIdByKey;
using Lucy.Console.Commands.Update;
using Lucy.Console.Enums;
using Lucy.Domain.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;
using AppUpdateTagCommand = Lucy.Application.Tags.Commands.UpdateTag.UpdateTagCommand;

namespace Lucy.Console.Tests.Commands.Update;

public class UpdateTagCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly UpdateTagCommandHandler _handler;

    public UpdateTagCommandHandlerTests()
    {
        _console = new TestConsole();
        _mediatorMock = new Mock<IMediator>();
        _localizerMock = new Mock<IStringLocalizer<Program>>();

        _localizerMock
            .Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()])
            .Returns((string key, object[] args) => new LocalizedString(key, $"{key} {string.Join(' ', args)}"));

        _handler = new UpdateTagCommandHandler(
            _console,
            _mediatorMock.Object,
            _localizerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithKeys_ResolvesAndUpdates()
    {
        var command = new UpdateTagCommand
        {
            ProjectKey = "EXAMP",
            TagKey = "BUG",
            ProjectId = null,
            TagId = null,
            NewKey = "BUG2",
            Label = "Bug",
            Description = "Desc",
            Color = Color.Red
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.ProjectKey), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetTagIdByKeyQuery>(q => q.ProjectId == 1 && q.Key == command.TagKey), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5L);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AppUpdateTagCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.UpdatedTag", _console.Output);
        Assert.Contains("5", _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<AppUpdateTagCommand>(c =>
                    c.Id == 5 &&
                    c.Key == command.NewKey &&
                    c.Label == command.Label &&
                    c.Description == command.Description &&
                    c.Color == command.Color),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithId_SkipsKeyResolution()
    {
        var command = new UpdateTagCommand
        {
            ProjectKey = null,
            TagKey = null,
            ProjectId = 1,
            TagId = 7,
            NewKey = null,
            Label = "Label",
            Description = null,
            Color = null
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AppUpdateTagCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.UpdatedTag", _console.Output);
        Assert.Contains("7", _console.Output);

        _mediatorMock.Verify(
            m => m.Send(It.Is<GetProjectIdByKeyQuery>(q => true), It.IsAny<CancellationToken>()),
            Times.Never);
        _mediatorMock.Verify(
            m => m.Send(It.Is<GetTagIdByKeyQuery>(q => true), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithPositionalTagKey_UsesProjectId()
    {
        var command = new UpdateTagCommand
        {
            ProjectKey = "BUG",
            TagKey = null,
            ProjectId = 3,
            TagId = null,
            NewKey = "NEWBUG",
            Label = null,
            Description = "desc",
            Color = Color.Blue
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetTagIdByKeyQuery>(q => q.ProjectId == 3 && q.Key == "BUG"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(15L);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AppUpdateTagCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.UpdatedTag", _console.Output);
        Assert.Contains("15", _console.Output);

        _mediatorMock.Verify(
            m => m.Send(It.Is<GetProjectIdByKeyQuery>(q => true), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
