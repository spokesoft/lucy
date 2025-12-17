using AppUpdateCommentCommand = Lucy.Application.Comments.Commands.UpdateComment.UpdateCommentCommand;
using Lucy.Application.Common.Interfaces;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Handler for the <see cref="UpdateCommentCommand"/> command.
/// </summary>
internal class UpdateCommentCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<UpdateCommentCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        UpdateCommentCommand command,
        CancellationToken token = default)
    {
        var request = new AppUpdateCommentCommand(
            command.Id,
            command.Content!);

        await _mediator.Send(request, token);

        _console.MarkupLine("[green]:check_mark:[/] " + _localizer["Messages.UpdatedComment", command.Id]);

        return ExitCode.Success;
    }
}
