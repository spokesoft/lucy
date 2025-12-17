using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tags.Queries.GetTagIdByKey;
using Lucy.Console.Commands.Delete;
using Lucy.Console.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;
using AppDeleteTagCommand = Lucy.Application.Tags.Commands.DeleteTag.DeleteTagCommand;

namespace Lucy.Console.Tests.Commands.Delete;

public class DeleteTagCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DeleteTagCommandHandler _handler;

    public DeleteTagCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _mediatorMock = new Mock<IMediator>();

        _localizerMock
            .Setup(x => x[It.IsAny<string>(), It.IsAny<object[]>()])
            .Returns((string key, object[] args) => new LocalizedString(key, $"{key} {string.Join(' ', args)}"));

        _handler = new DeleteTagCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithKeys_ResolvesAndDeletes()
    {
        var command = new DeleteTagCommand
        {
            ProjectKey = "EXAMP",
            TagKey = "BUG",
            ProjectId = null,
            TagId = null
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.ProjectKey), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetTagIdByKeyQuery>(q => q.ProjectId == 1 && q.Key == command.TagKey), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5L);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AppDeleteTagCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.DeletedTag", _console.Output);
        Assert.Contains("5", _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<AppDeleteTagCommand>(c => c.Id == 5),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithIds_SkipsLookups()
    {
        var command = new DeleteTagCommand
        {
            ProjectKey = null,
            TagKey = null,
            ProjectId = 2,
            TagId = 9
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AppDeleteTagCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.DeletedTag", _console.Output);
        Assert.Contains("9", _console.Output);

        _mediatorMock.Verify(
            m => m.Send(It.Is<GetProjectIdByKeyQuery>(q => true), It.IsAny<CancellationToken>()),
            Times.Never);

        _mediatorMock.Verify(
            m => m.Send(It.Is<GetTagIdByKeyQuery>(q => true), It.IsAny<CancellationToken>()),
            Times.Never);

        _mediatorMock.Verify(
            m => m.Send(It.Is<AppDeleteTagCommand>(c => c.Id == 9), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithPositionalTagKey_UsesProjectId()
    {
        var command = new DeleteTagCommand
        {
            ProjectKey = "BUG",
            TagKey = null,
            ProjectId = 3,
            TagId = null
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetTagIdByKeyQuery>(q => q.ProjectId == 3 && q.Key == "BUG"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(11L);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AppDeleteTagCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.DeletedTag", _console.Output);
        Assert.Contains("11", _console.Output);

        _mediatorMock.Verify(
            m => m.Send(It.Is<GetProjectIdByKeyQuery>(q => true), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
