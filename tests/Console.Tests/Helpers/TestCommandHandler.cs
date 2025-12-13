using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Tests.Helpers;

/// <summary>
/// Test command handler for unit testing purposes.
/// </summary>
public class TestCommandHandler : ICommandHandler<TestCommand>
{
    public bool WasCalled { get; private set; }
    public TestCommand? LastCommand { get; private set; }
    public CommandContext? LastContext { get; private set; }
    public ExitCode ReturnValue { get; set; } = ExitCode.Success;
    public Exception? ExceptionToThrow { get; set; }
    public TimeSpan Delay { get; set; } = TimeSpan.Zero;

    public async Task<ExitCode> HandleAsync(CommandContext context, TestCommand command, CancellationToken token = default)
    {
        WasCalled = true;
        LastCommand = command;
        LastContext = context;

        if (Delay > TimeSpan.Zero)
        {
            await Task.Delay(Delay, token);
        }

        if (ExceptionToThrow != null)
        {
            throw ExceptionToThrow;
        }

        return ReturnValue;
    }

    public void Reset()
    {
        WasCalled = false;
        LastCommand = null;
        LastContext = null;
        ReturnValue = ExitCode.Success;
        ExceptionToThrow = null;
        Delay = TimeSpan.Zero;
    }
}
