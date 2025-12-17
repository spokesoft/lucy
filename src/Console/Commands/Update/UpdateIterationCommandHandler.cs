using AppUpdateIterationCommand = Lucy.Application.Iterations.Commands.UpdateIteration.UpdateIterationCommand;
using Lucy.Application.Common.Interfaces;
using Lucy.Application.Iterations.Queries.GetIterationByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Handler for the <see cref="UpdateIterationCommand"/> command.
/// </summary>
internal class UpdateIterationCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<UpdateIterationCommand>
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
        UpdateIterationCommand command,
        CancellationToken token = default)
    {
        // Resolve iteration ID from key if needed
        long iterationId;
        if (command.Id.HasValue)
        {
            iterationId = command.Id.Value;
        }
        else
        {
            var iterationQuery = new GetIterationByKeyQuery(command.Key!);
            var iteration = await _mediator.Send(iterationQuery, token);

            iterationId = iteration!.Id;
        }

        var request = new AppUpdateIterationCommand(
            iterationId,
            command.Name,
            command.Description,
            command.StartDate,
            command.EndDate);

        await _mediator.Send(request, token);

        _console.MarkupLine("[green]:check_mark:[/] " + _localizer["Messages.UpdatedIteration", iterationId]);

        return ExitCode.Success;
    }
}
