using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Settings for the 'update project' command.
/// </summary>
public class UpdateProjectCommand : UpdateCommand
{
    /// <summary>
    /// The key of the project to update.
    /// </summary>
    [CommandArgument(0, "[key]")]
    [Description("The key of the project to update.")]
    public required string? Key { get; set; }

    /// <summary>
    /// The ID of the project to update.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the project to update.")]
    public required long? Id { get; set; }

    /// <summary>
    /// Name of the project.
    /// </summary>
    [CommandOption("-n|--name <NAME>")]
    [Description("Name of the project.")]
    public string? Name { get; set; }

    /// <summary>
    /// Description of the project.
    /// </summary>
    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("Description of the project.")]
    public string? Description { get; set; }
}
