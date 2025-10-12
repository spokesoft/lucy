using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Settings for the 'new project' command.
/// </summary>
public class NewProjectCommand : NewCommand
{
    /// <summary>
    /// The unique key for the project.
    /// </summary>
    [CommandArgument(0, "<key>")]
    [Description("The unique key for the project.")]
    public required string Key { get; set; }

    /// <summary>
    /// The name of the project.
    /// </summary>
    [CommandOption("-n|--name <NAME>")]
    [Description("The name of the project.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the project.
    /// </summary>
    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("The description of the project.")]
    public string Description { get; set; } = string.Empty;
}
