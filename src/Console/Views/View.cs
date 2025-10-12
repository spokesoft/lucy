using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Lucy.Console.Views;

public abstract class View<TState>(
    TState state,
    IStringLocalizer localizer,
    IAnsiConsole? console = null) : Renderable
    where TState : ViewOptions
{
    protected readonly TState _state = state;
    protected readonly IStringLocalizer _localizer = localizer;
    protected readonly IAnsiConsole _console = console ?? AnsiConsole.Console;
}
