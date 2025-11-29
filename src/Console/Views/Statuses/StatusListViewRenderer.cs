using Lucy.Application.Statuses.DTOs;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;

using DomainColor = Lucy.Domain.Enums.Color;

namespace Lucy.Console.Views.Statuses;

/// <summary>
/// Renders the status list view to the console.
/// </summary>
public class StatusListViewRenderer : IViewRenderer<IEnumerable<StatusDto>>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        IEnumerable<StatusDto> model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var statusList = model.ToList();
        var title = GetTitle(localizer, console.Profile);
        var caption = statusList.Any()
            ? GetCaption(localizer, 1, statusList.Count, statusList.Count)
            : localizer["View.StatusList.NoStatuses"];

        var table = new Table()
            .Border(TableBorder.SimpleHeavy)
            .Title(title)
            .Caption(caption, new Style(foreground: Color.Grey))
            .Width(80);

        table.AddColumn(localizer["Property.Id"]);
        table.AddColumn("#"); // Use '#' for table column header
        table.AddColumn(localizer["Property.Status.Key"]);
        table.AddColumn(localizer["Property.Status.Name"]);
        table.AddColumn(localizer["Property.Status.Description"]);

        foreach (var status in statusList)
        {
            var name = string.IsNullOrWhiteSpace(status.Name)
                ? $"[grey70]{localizer["Empty.String"]}[/]"
                : status.Name;

            var description = string.IsNullOrWhiteSpace(status.Description)
                ? $"[grey70]{localizer["Empty.String"]}[/]"
                : status.Description;

            var coloredKey = GetColoredKey(status.Key, status.Color);

            table.AddRow(
                $"[gray]{status.Id}[/]",
                status.Order.ToString(),
                coloredKey,
                name,
                description);
        }

        console.WriteLine();
        console.Write(table);
        console.WriteLine();

        await Task.CompletedTask;
    }

    /// <summary>
    /// Gets the title for the status list view.
    /// </summary>
    private static string GetTitle(IStringLocalizer localizer, Profile options)
        => options.Capabilities.Unicode
            ? $":clipboard: [u]{localizer["View.StatusList.Title"]}[/]"
            : $"[u]{localizer["View.StatusList.Title"]}[/]";

    /// <summary>
    /// Gets the caption for the status list view.
    /// </summary>
    private static string GetCaption(IStringLocalizer localizer, int firstIndex, int lastIndex, int count)
        => count == 0 ?
            localizer["View.StatusList.NoStatuses"] :
            localizer["View.StatusList.Caption", firstIndex, lastIndex, count];

    /// <summary>
    /// Gets the colored key display markup for a status.
    /// </summary>
    private static string GetColoredKey(string key, DomainColor color)
    {
        var colorName = color.ToString().ToLower();
        return color switch
        {
            DomainColor.Red => $"[red]{key}[/]",
            DomainColor.Orange => $"[orangered1]{key}[/]",
            DomainColor.Yellow => $"[yellow]{key}[/]",
            DomainColor.Green => $"[green]{key}[/]",
            DomainColor.Blue => $"[blue]{key}[/]",
            DomainColor.Purple => $"[purple]{key}[/]",
            DomainColor.Gray => $"[gray]{key}[/]",
            _ => key
        };
    }
}
