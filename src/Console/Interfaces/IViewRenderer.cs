using Microsoft.Extensions.Localization;
using Spectre.Console;

namespace Lucy.Console.Interfaces;

public interface IViewRenderer<TModel>
{
    /// <summary>
    /// Renders the specified model to the console.
    /// </summary>
    Task RenderAsync(
        TModel model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default);
}
