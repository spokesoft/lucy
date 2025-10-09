using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Settings for the 'show project' command.
/// </summary>
public class ShowProjectCommand : ShowCommand
{
    /// <summary>
    /// Unique key of the project to show.
    /// </summary>
    [CommandArgument(0, "[KEY]")]
    [Description("Unique key of the project to show.")]
    public string? Key { get; set; }
}
