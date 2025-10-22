using Lucy.Application.Projects.DTOs;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;

namespace Lucy.Console.Views.Projects;

/// <summary>
/// Renders the project view to the console.
/// </summary>
public class ProjectViewRenderer : IViewRenderer<ProjectDto>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        ProjectDto model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var title = GetTitle(localizer, console.Profile);
        var content = GetProjectDetailsMarkup(model, localizer);

        var panel = new Panel(new Markup(content))
            .Header(title, Justify.Center)
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("grey"))
            .Padding(1, 2);

        console.Write(panel);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Gets the title for the project view.
    /// </summary>
    private static string GetTitle(IStringLocalizer localizer, Profile options)
        => options.Capabilities.Unicode
            ? $":file_folder: [u]{localizer["View.ShowProject.Title"]}[/]"
            : $"[u]{localizer["View.ShowProject.Title"]}[/]";

    /// <summary>
    /// Gets the project details formatted as markup.
    /// </summary>
    private static string GetProjectDetailsMarkup(ProjectDto project, IStringLocalizer localizer)
    {
        var name = string.IsNullOrWhiteSpace(project.Name)
            ? $"[grey70]{localizer["Empty.String"]}[/]"
            : project.Name;

        var description = string.IsNullOrWhiteSpace(project.Description)
            ? $"[grey70]{localizer["Empty.String"]}[/]"
            : project.Description;

        return $"[grey]{localizer["Property.Id"]}:[/] {project.Id}\n" +
               $"[grey]{localizer["Property.Project.Key"]}:[/] [blue]{project.Key}[/]\n" +
               $"[grey]{localizer["Property.Project.Name"]}:[/] {name}\n" +
               $"[grey]{localizer["Property.Project.Description"]}:[/] {description}\n" +
               $"[grey]{localizer["Property.CreatedAt"]}:[/] {project.CreatedAt:yyyy-MM-dd HH:mm:ss}\n" +
               $"[grey]{localizer["Property.UpdatedAt"]}:[/] {project.UpdatedAt:yyyy-MM-dd HH:mm:ss}";
    }
}
