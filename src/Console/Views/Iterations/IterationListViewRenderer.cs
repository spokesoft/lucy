using Lucy.Application.Iterations.DTOs;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;

namespace Lucy.Console.Views.Iterations;

/// <summary>
/// Renders the iteration list view to the console.
/// </summary>
public class IterationListViewRenderer : IViewRenderer<IEnumerable<IterationDto>>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        IEnumerable<IterationDto> model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var title = GetTitle(localizer, console.Profile);
        var caption = model.Any()
            ? GetCaption(localizer, 1, model.Count(), model.Count())
            : localizer["View.IterationList.NoIterations"];

        var table = new Table()
            .Border(TableBorder.SimpleHeavy)
            .Title(title)
            .Caption(caption, new Style(foreground: Color.Grey))
            .Width(80);

        table.AddColumn(localizer["Property.Id"]);
        table.AddColumn(localizer["Property.Iteration.Key"]);
        table.AddColumn(localizer["Property.Iteration.Name"]);
        table.AddColumn(localizer["Property.Iteration.StartDate"]);
        table.AddColumn(localizer["Property.Iteration.EndDate"]);

        foreach (var iteration in model)
        {
            var name = string.IsNullOrWhiteSpace(iteration.Name)
                ? $"[grey70]{localizer["Empty.String"]}[/]"
                : iteration.Name;

            var startDate = iteration.StartDate.HasValue
                ? iteration.StartDate.Value.ToShortDateString()
                : $"[grey70]{localizer["Empty.String"]}[/]";

            var endDate = iteration.EndDate.HasValue
                ? iteration.EndDate.Value.ToShortDateString()
                : $"[grey70]{localizer["Empty.String"]}[/]";

            table.AddRow(
                $"[gray]{iteration.Id}[/]",
                $"[blue]{iteration.Key}[/]",
                name,
                startDate,
                endDate);
        }

        console.WriteLine();
        console.Write(table);
        console.WriteLine();
    }

    private static string GetTitle(IStringLocalizer localizer, Profile options)
        => options.Capabilities.Unicode
            ? $":calendar: [u]{localizer["View.IterationList.Title"]}[/]"
            : $"[u]{localizer["View.IterationList.Title"]}[/]";

    private static string GetCaption(IStringLocalizer localizer, int page, int pageSize, int totalCount)
    {
        return localizer["View.IterationList.Caption", page, pageSize, totalCount];
    }
}
