using AppDeleteCommentCommand = Lucy.Application.Comments.Commands.DeleteComment.DeleteCommentCommand;
using Lucy.Application.Interfaces;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Handler for the <see cref="DeleteCommentCommand"/> command.
/// </summary>
internal class DeleteCommentCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<DeleteCommentCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        DeleteCommentCommand command,
        CancellationToken token = default)
    {
        var request = new AppDeleteCommentCommand(command.Id);
        await _mediator.Send(request, token);

        _console.MarkupLine("[green]:check_mark:[/] " + _localizer["Messages.DeletedComment", command.Id]);

        return ExitCode.Success;
    }
}
