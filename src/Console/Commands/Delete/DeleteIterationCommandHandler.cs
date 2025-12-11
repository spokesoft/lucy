using AppDeleteIterationCommand = Lucy.Application.Iterations.Commands.DeleteIteration.DeleteIterationCommand;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;
using Lucy.Application.Iterations.Queries.GetIterationIdByKey;
using Lucy.Application.Interfaces;
using Spectre.Console;
using Microsoft.Extensions.Localization;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Handler for the <see cref="DeleteIterationCommand"/> command.
/// </summary>
internal class DeleteIterationCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<DeleteIterationCommand>
{
    /// <summary>
    /// The console instance for outputting information.
    /// </summary>
    private readonly IAnsiConsole _console = console;

    /// <summary>
    /// The localizer instance for localized strings.
    /// </summary>
    private readonly IStringLocalizer<Program> _localizer = localizer;

    /// <summary>
    /// The mediator instance for sending commands and queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        DeleteIterationCommand command,
        CancellationToken token = default)
    {
        var iterationId = command.Id;
        if (iterationId is null)
        {
            var query = new GetIterationIdByKeyQuery(command.Key!);
            iterationId = await _mediator.Send(query, token);
        }

        var request = new AppDeleteIterationCommand(iterationId.Value);
        await _mediator.Send(request, token);

        _console.MarkupLine("[green]:check_mark:[/] " + _localizer["Messages.DeletedIteration", iterationId]);

        return ExitCode.Success;
    }
}
