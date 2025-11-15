using Lucy.Application.Statuses.DTOs;
using Lucy.Console.Interfaces;
using Lucy.Domain.Enums;
using Microsoft.Extensions.Localization;
using Spectre.Console;

namespace Lucy.Console.Views.Statuses;

/// <summary>
/// Renders the status view to the console.
/// </summary>
public class StatusViewRenderer : IViewRenderer<StatusDto>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        StatusDto model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var title = GetTitle(localizer, console.Profile);
        var content = GetStatusDetailsMarkup(model, localizer);

        var panel = new Panel(new Markup(content))
            .Header(title, Justify.Center)
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("grey"))
            .Padding(1, 2);

        console.Write(panel);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Gets the title for the status view.
    /// </summary>
    private static string GetTitle(IStringLocalizer localizer, Profile options)
        => options.Capabilities.Unicode
            ? $":bookmark: [u]{localizer["View.ShowStatus.Title"]}[/]"
            : $"[u]{localizer["View.ShowStatus.Title"]}[/]";

    /// <summary>
    /// Gets the status details formatted as markup.
    /// </summary>
    private static string GetStatusDetailsMarkup(StatusDto status, IStringLocalizer localizer)
    {
        var name = string.IsNullOrWhiteSpace(status.Name)
            ? $"[grey70]{localizer["Empty.String"]}[/]"
            : status.Name;

        var description = string.IsNullOrWhiteSpace(status.Description)
            ? $"[grey70]{localizer["Empty.String"]}[/]"
            : status.Description;

        var colorDisplay = GetColorDisplay(status.Color);

        return $"[grey]{localizer["Property.Id"]}:[/] {status.Id}\n" +
               $"[grey]{localizer["Property.Status.ProjectId"]}:[/] {status.ProjectId}\n" +
               $"[grey]{localizer["Property.Status.Key"]}:[/] [blue]{status.Key}[/]\n" +
               $"[grey]{localizer["Property.Status.Order"]}:[/] {status.Order}\n" +
               $"[grey]{localizer["Property.Status.Name"]}:[/] {name}\n" +
               $"[grey]{localizer["Property.Status.Description"]}:[/] {description}\n" +
               $"[grey]{localizer["Property.Status.Color"]}:[/] {colorDisplay}\n" +
               $"[grey]{localizer["Property.CreatedAt"]}:[/] {status.CreatedAt:yyyy-MM-dd HH:mm:ss}\n" +
               $"[grey]{localizer["Property.UpdatedAt"]}:[/] {status.UpdatedAt:yyyy-MM-dd HH:mm:ss}";
    }

    /// <summary>
    /// Gets the color display markup for a status color.
    /// </summary>
    private static string GetColorDisplay(StatusColor color)
    {
        var colorName = color.ToString().ToLower();
        return color switch
        {
            StatusColor.Red => $"[red]●[/] {colorName}",
            StatusColor.Orange => $"[orangered1]●[/] {colorName}",
            StatusColor.Yellow => $"[yellow]●[/] {colorName}",
            StatusColor.Green => $"[green]●[/] {colorName}",
            StatusColor.Blue => $"[blue]●[/] {colorName}",
            StatusColor.Purple => $"[purple]●[/] {colorName}",
            StatusColor.Gray => $"[gray]●[/] {colorName}",
            _ => colorName
        };
    }
}
