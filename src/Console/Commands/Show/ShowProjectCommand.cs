using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Settings for the 'show project' command.
/// </summary>
public class ShowProjectCommand : ShowCommand
{
    /// <summary>
    /// The key of the project to show.
    /// </summary>
    [CommandArgument(0, "[key]")]
    [Description("The key of the project to show.")]
    public required string? Key { get; set; }

    /// <summary>
    /// The ID of the project to show.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the project to show.")]
    public required long? Id { get; set; }
}
