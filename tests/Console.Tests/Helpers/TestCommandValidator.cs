using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Tests.Helpers;

/// <summary>
/// Test command validator for unit testing purposes.
/// </summary>
public class TestCommandValidator : ICommandValidator<TestCommand>
{
    public bool WasCalled { get; private set; }
    public TestCommand? LastCommand { get; private set; }
    public CommandContext? LastContext { get; private set; }
    public ValidationResult ResultToReturn { get; set; } = new();
    public Exception? ExceptionToThrow { get; set; }
    public TimeSpan Delay { get; set; } = TimeSpan.Zero;

    public async Task<ValidationResult> ValidateAsync(CommandContext context, TestCommand settings, CancellationToken token = default)
    {
        WasCalled = true;
        LastCommand = settings;
        LastContext = context;

        if (Delay > TimeSpan.Zero)
        {
            await Task.Delay(Delay, token);
        }

        if (ExceptionToThrow != null)
        {
            throw ExceptionToThrow;
        }

        return ResultToReturn;
    }

    public void Reset()
    {
        WasCalled = false;
        LastCommand = null;
        LastContext = null;
        ResultToReturn = new ValidationResult();
        ExceptionToThrow = null;
        Delay = TimeSpan.Zero;
    }
}
