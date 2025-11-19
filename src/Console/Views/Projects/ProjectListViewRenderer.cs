using Lucy.Application.Projects.DTOs;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;

namespace Lucy.Console.Views.Projects;

/// <summary>
/// Renders the project list view to the console.
/// </summary>
public class ProjectListViewRenderer : IViewRenderer<IEnumerable<ProjectDto>>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        IEnumerable<ProjectDto> model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var title = GetTitle(localizer, console.Profile);
        var caption = model.Any()
            ? GetCaption(localizer, 1, model.Count(), model.Count())
            : localizer["View.ProjectList.NoProjects"];

        var table = new Table()
            .Border(TableBorder.SimpleHeavy)
            .Title(title)
            .Caption(caption, new Style(foreground: Color.Grey))
            .Width(80);

        table.AddColumn(localizer["Property.Id"]);
        table.AddColumn(localizer["Property.Project.Key"]);
        table.AddColumn(localizer["Property.Project.Name"]);
        table.AddColumn(localizer["Property.Project.Description"]);
        table.AddColumn(localizer["Property.UpdatedAt"]);

        foreach (var project in model)
        {
            var name = string.IsNullOrWhiteSpace(project.Name)
                ? $"[grey70]{localizer["Empty.String"]}[/]"
                : project.Name;

            var description = string.IsNullOrWhiteSpace(project.Description)
                ? $"[grey70]{localizer["Empty.String"]}[/]"
                : project.Description;

            table.AddRow(
                $"[gray]{project.Id}[/]",
                $"[blue]{project.Key}[/]",
                name,
                description,
                project.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        console.Write(table);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Gets the title for the project list view.
    /// </summary>
    private static string GetTitle(IStringLocalizer localizer, Profile options)
        => options.Capabilities.Unicode
            ? $":file_folder: [u]{localizer["View.ProjectList.Title"]}[/]"
            : $"[u]{localizer["View.ProjectList.Title"]}[/]";

    /// <summary>
    /// Gets the caption for the project list view.
    /// </summary>
    private static string GetCaption(IStringLocalizer localizer, int firstIndex, int lastIndex, int count)
        => count == 0 ?
            localizer["View.ProjectList.NoProjects"] :
            localizer["View.ProjectList.Caption", firstIndex, lastIndex, count];
}
