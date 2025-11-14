using Lucy.Application.Statuses.DTOs;
using Lucy.Console.Interfaces;
using Lucy.Domain.Enums;
using Microsoft.Extensions.Localization;
using Spectre.Console;

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
            .Caption(caption, new Style(foreground: Color.Grey));

        table.AddColumn(localizer["Property.Id"]);
        table.AddColumn(localizer["Property.Status.Key"]);
        table.AddColumn(localizer["Property.Status.Order"]);
        table.AddColumn(localizer["Property.Status.Name"]);
        table.AddColumn(localizer["Property.Status.Description"]);
        table.AddColumn(localizer["Property.Status.Color"]);
        table.AddColumn(localizer["Property.UpdatedAt"]);

        foreach (var status in statusList)
        {
            var name = string.IsNullOrWhiteSpace(status.Name)
                ? $"[grey70]{localizer["Empty.String"]}[/]"
                : status.Name;

            var description = string.IsNullOrWhiteSpace(status.Description)
                ? $"[grey70]{localizer["Empty.String"]}[/]"
                : status.Description;

            var colorDisplay = GetColorDisplay(status.Color);

            table.AddRow(
                $"[gray]{status.Id}[/]",
                $"[blue]{status.Key}[/]",
                status.Order.ToString(),
                name,
                description,
                colorDisplay,
                status.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        console.Write(table);
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
    /// Gets the color display markup for a status color.
    /// </summary>
    private static string GetColorDisplay(StatusColor color)
    {
        var colorName = color.ToString().ToLower();
        return color switch
        {
            StatusColor.Red => $"[red]{colorName}[/]",
            StatusColor.Orange => $"[orangered1]{colorName}[/]",
            StatusColor.Yellow => $"[yellow]{colorName}[/]",
            StatusColor.Green => $"[green]{colorName}[/]",
            StatusColor.Blue => $"[blue]{colorName}[/]",
            StatusColor.Purple => $"[purple]{colorName}[/]",
            StatusColor.Gray => $"[gray]{colorName}[/]",
            _ => colorName
        };
    }
}
